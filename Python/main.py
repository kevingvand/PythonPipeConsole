import struct
import time

pipe_name = 'XXX'
encoding = 'ascii'


def connect(path, attempt):
    try:
        if attempt > 100:
            print("Could not connect to pipe, timeout")
            return

        server = open(path, 'r+b', 0)
        server.seek(0)
        print("Connected")

        return server
    except:
        time.sleep(.1)
        connect(path, attempt + 1)


def write_message(pipe, message):
    pipe.write(message)
    pipe.seek(0)


def read_message(pipe):
    type_bytes = pipe.read(1)

    if not type_bytes.__len__() is 1:
        pipe.close()
        return

    length_bytes = pipe.read(2)
    length = length_bytes[0] * 256 + length_bytes[1]
    message = pipe.read(length).decode(encoding)
    pipe.seek(0)
    process_message(pipe, type_bytes[0], message)


def get_length_bytes(length):
    return struct.pack("B B", length // 256, length & 255)


def write_error(pipe, error):
    error_message = error.encode(encoding)
    error_bytes = struct.pack("B", 4) + get_length_bytes(error_message.__len__()) + error_message
    write_message(pipe, error_bytes)


def read_array(result):
    array_length = result.__len__()
    if array_length is 0:
        return [bytes(0), 32]

    value_bytes, result_type = get_bytes(result[0])
    value_bytes = get_length_bytes(value_bytes.__len__()) + value_bytes

    for i in range(1, array_length):
        element_bytes = get_bytes(result[i])[0]
        value_bytes += get_length_bytes(element_bytes.__len__()) + element_bytes

    value_bytes = get_length_bytes(array_length) + value_bytes

    return [value_bytes, 32 | result_type]


def get_bytes(value):
    if isinstance(value, list):
        value_bytes, result_type = read_array(value)
    elif isinstance(value, float):
        value_bytes = struct.pack("f", value)
        result_type = 4
    elif type(value) is bool:
        value_bytes = struct.pack("?", value)
        result_type = 2
    elif isinstance(value, int):
        value_bytes = struct.pack("i", value)
        result_type = 1
    else:
        value_bytes = str(value).encode(encoding)
        result_type = 16

    return [value_bytes, result_type]


def evaluate_message(pipe, message):
    try:
        result = eval(message, globals())
    except Exception as e:
        write_error(pipe, str(e))
        return

    value_bytes, result_type = get_bytes(result)
    result_bytes = struct.pack("B B", 3, result_type) + get_length_bytes(value_bytes.__len__()) + value_bytes
    write_message(pipe, result_bytes)


def execute_message(pipe, message):
    try:
        exec(message, globals())
    except Exception as e:
        write_error(pipe, str(e))
        return

    write_message(pipe, struct.pack("B B B B ?", 3, 2, 0, 1, True))


def process_message(pipe, type, message):
    if pipe.closed:
        return

    if type is 1: #Evaluate
        evaluate_message(pipe, message)
    if type is 2: #Execute
        execute_message(pipe, message)


if __name__ == '__main__':
    pipeRoot = R'\\.\pipe'
    pipePath = pipeRoot + '\\' + pipe_name

    print("Waiting for server...")

    server = connect(pipePath, 0)

    if server is None:
        print("Invalid Connection, shutting down...")
        exit(0)

    while True:
        if server.closed:
            print("Connection lost, shutting down...")
            exit(0)

        read_message(server)
