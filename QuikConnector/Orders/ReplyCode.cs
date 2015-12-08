/*
The MIT License (MIT)

Copyright (c) 2015 Denis Lebedev

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
 */

namespace QuikConnector.Orders
{
    //0 – транзакция отправлена серверу,  
    //1 – транзакция получена на сервер QUIK от клиента, 
    //2 – ошибка при передаче транзакции в торговую систему, поскольку отсутствует подключение шлюза Московской Биржи, повторно транзакция не отправляется, 
    //3 – транзакция выполнена, 
    //4 – транзакция не выполнена торговой системой, код ошибки торговой системы будет указан в поле «DESCRIPTION»,
    //5 – транзакция не прошла проверку сервера QUIK по каким-либо критериям. Например, проверку на наличие прав у пользователя на отправку транзакции данного типа,  
    //6 – транзакция не прошла проверку лимитов сервера QUIK, 
    //10 – транзакция не поддерживается торговой системой. К примеру, попытка отправить «ACTION = MOVE_ORDERS» на Московской Бирже, 
    //11 – транзакция не прошла проверку правильности электронной подписи. К примеру, если ключи, зарегистрированные на сервере, не соответствуют подписи отправленной транзакции 
    //12 – не удалось дождаться ответа на транзакцию, т.к. истек таймаут ожидания. Может возникнуть при подаче транзакций из QPILE 
    //13 – транзакция отвергнута, т.к. ее выполнение могло привести к кросс-сделке (т.е. сделке с тем же самым клиентским счетом) 


    public enum ReplyCode
    {
        SentToServer = 0,
        ReceivedOnServer = 1,
        GatewayError = 2,
        Executed = 3,
        NotExecutedByTradeSystem = 4,
        VerifiedFail = 5,
        VerifiedLimitsFail = 6,
        NotSupportedByTradeSystem = 10,
        ElectronicSignatureError = 11,
        ExpiredTimeout = 12,
        CrossTradeError = 13
    }
}
