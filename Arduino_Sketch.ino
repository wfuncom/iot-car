//以下数字都是Arduino上的引脚编号，你可以看到合计有四个引脚需要在代码里面用到了，而且每个引脚可以同时设置为数字/模拟端口。
const int kong_in1 = 5; //数字端口 只有开和关 （所谓数字，你可以理解为把物理上的低电平和高电平转换为编程上的0和1两个数值，再简单点理解就是开关，只不过这里的开关在关闭状态下仍然是有电流在流入的。）
const int kong_in2 = 6; //数字端口  只有开和关
const int kong_in3 = 9; //数字端口  只有开和关
const int kong_in4 = 10; //数字端口  只有开和关
const int pwm_in1 = 5; //模拟端口  数值范围0~255（所谓模拟，可以理解为把电子元件的物理值转换为范围为编程上的0~255的数值，比如电机的转速就需要这个来控制）
const int pwm_in2 = 6; //模拟端口 
const int pwm_in3 = 9;//模拟端口
const int pwm_in4 = 10;//模拟端口
 
int l_speed = 0;//定义速度变量，PWM输出范围为0～255
int l_pwm_in;
int r_speed = 0;//定义速度变量，PWM输出范围为0～255
int r_pwm_in;
 
int gs = 0;
//先初始化，可以理解为Arduino在启动之后需要执行的代码
void setup() {
    Serial.begin(115200);  // 115200是蓝牙通信过程蓝牙接收器元件需要用到的的端口号，有的蓝牙模块可能和我这个不一样。
    pinMode(kong_in1,OUTPUT); //模拟端口默认设置为输出，这个用来控制电机速度
    pinMode(kong_in2,OUTPUT);
    digitalWrite(kong_in1, LOW); //数字端口默认设置为低电平，也就是先关闭电机
    digitalWrite(kong_in2, LOW);
    pinMode(kong_in3, OUTPUT);
    pinMode(kong_in4, OUTPUT);
    digitalWrite(kong_in3, LOW);
    digitalWrite(kong_in4, LOW);
     
}
 
// Arduino在运行的时候会循环的执行这里面的代码
void loop() {   
     
    while (Serial.available())
    {
        char c = Serial.read();//读取蓝牙接收到的字符串
        Serial.println(c);
        if (c == 'q') {  // 如果给蓝牙串口传一个'q'字符表示左侧轮子前进
            l_speed = 150;          //先设定一个可以控制转速的模拟数值为150，注意这里只是编程值，不是转速也不是电压值/电流值。
            l_pwm_in = pwm_in1;         //把这个数值传给1号模拟口
            //以下一高一低，就让左侧电机正转
            digitalWrite(kong_in1, HIGH);    //给1号端口一个高电平
            digitalWrite(kong_in2, LOW);     //给2号端口一个低电平
             
             
        }
 
        if (c == 'a') {  // 如果给蓝牙串口传一个'a'字符表示左侧轮子后退
            l_speed = 150;              //先设定一个可以控制转速的模拟数值为150，注意这里只是编程值，不是转速也不是电压值/电流值。
            l_pwm_in = pwm_in2;         //把这个数值传给2号模拟口
            //以下一低一高，就让左侧电机反转
            digitalWrite(kong_in1, LOW);  //给1号端口一个低电平
            digitalWrite(kong_in2, HIGH);  //给2号端口一个高电平
             
 
        }
        if (c == 'z') {  // 如果给蓝牙串口传一个'z'字符表示左侧轮子停止
 
            //以下表示电机速度为0，而去两个相同电平，表示左侧电机停转
            l_speed = 0;            
            digitalWrite(kong_in1, LOW);
            digitalWrite(kong_in2, LOW);        
             
             
        }       
        //以下解释不多，因为与上面意思差不多
        if (c == 'w') {  // 如果给蓝牙串口传一个'w'字符表示右侧轮子前进
            r_speed = 150;
            r_pwm_in = pwm_in3;
            digitalWrite(kong_in3, HIGH);
            digitalWrite(kong_in4, LOW);
             
             
        }               
        if (c == 's') { // 如果给蓝牙串口传一个's'字符表示右侧轮子后退
            r_speed = 150;
            r_pwm_in = pwm_in4;
            digitalWrite(kong_in3, LOW);
            digitalWrite(kong_in4, HIGH);
        }
        if (c == 'x') { // 如果给蓝牙串口传一个'x'字符表示右侧轮子停止
            r_speed = 0;
            digitalWrite(kong_in3, LOW);
            digitalWrite(kong_in4, LOW);
        }
         
    }
    //左侧轮子开始转动
    if (l_speed > 0) { //速度为0时开始加速
        l_speed += 10;          //因为在循环里面，所以这里表示随时间加速   
    }
    if (l_speed > 250) {     //速度达到最大值250则不要再加了，防止电机发热。
        l_speed = 250;
    }
    //右侧轮子开始转动
    if (r_speed > 0) {
        r_speed += 10;
    }
    if (r_speed > 250) {     
        r_speed = 250;
    }
    delay(100); // 循环一次停100毫秒，因为循环太快的话，加速效果看不到了。
    if (l_pwm_in) { //结合上面代码，这里用来判断是前进还是后退
        analogWrite(l_pwm_in, l_speed); // 模拟端口向左边电机输出速度值
    }
    if (r_pwm_in) { //结合上面代码，这里用来判断是前进还是后退
        analogWrite(r_pwm_in, r_speed); // 模拟端口向右边电机输出速度值
    }
}
