import http.server
import socketserver

flag = True
while(flag):
    print('请输入端口号(1024-65535),直接回车使用默认端口2333：')
    v = input()
    if(len(v)==0):
        port=2333
        break
    if(v.isdigit()):
        port = int(v)
        if(port > 1023 and port < 65536):
            flag = False
            break
    print('输入有误，请重新输入')
print('端口号为：'+str(port))

Handler =http.server.SimpleHTTPRequestHandler

with socketserver.TCPServer(("",port),Handler) as httpd:
    print("serving at port",port)
    print("http://127.0.0.1:"+str(port))
    httpd.serve_forever()