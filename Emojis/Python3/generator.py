import sys
def returnnum(number):
    number_list=[':zero: ', ':one: ', ':two: ', ':three: ', ':four: ', ':five: ', ':six: ', ':seven: ', ':eight: ', ':nine: ']
    return number_list[number]
def Emoji(text):
    emoji=''
    for letter in text:
        if letter.isalpha(): #숫자가 아닌 문자
            if 65<=ord(letter)<=90 or 97<=ord(letter)<=122: #알파벳
                emoji+=':regional_indicator_'+letter.lower()+': '
            else: #알파벳 이외의 문자
                emoji+=letter
        else: #숫자
            try:
                if 0<=int(letter)<=9:
                    emoji+=returnnum(int(letter))
                else:
                    emoji+=letter
            except ValueError: #숫자가 아니면서 isalpha() 통과하는 문자 걸러내기위함(백슬래시 등)
                if letter==' ': #공백은 너무 티가 안나서 2칸 띄어쓰기로 바꿔버리는걸루~
                    emoji+='  '
                else:
                    emoji+=letter
    print(emoji)

text=input()
if text=='' or text=='help' or text=='usage':
    sys.stdout.write('type \'file\' to generate emoji from file')
elif text=='file':
    filename=input('input text file name to generate discord emoji : ')
    try:
        f=open(filename, 'r')
        for buff in f.readlines():
            Emoji(buff)
    except IOError:
        sys.stdout.write("cannot open file '%s'\n" % filename)
else:
    Emoji(text)
