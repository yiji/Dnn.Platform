// index script
var oDepDate  = new Date();

var oBackDate = new Date(oDepDate.getFullYear(), oDepDate.getMonth(), oDepDate.getDate() + 2);

//出发时间实例	

var oDepCal = new Calendar({

		id: 		   "#lv_date_y",			//触发显示日历元素ID

		isPopup:       !0,					//弹出式日历

		isPrevBtn:     !0,					//显示上月按钮

		isNextBtn:     !0,					//显示下月按钮

		isCloseBtn:    !0,					//显示关闭按钮

		isHoliday:     !0,					//节假日特殊显示

		isHolidayTips: !0,					//显示节假日1~3天/后1~3天信息

		isDateInfo:    !0,					//显示日期信息

		isMessage: 	   !0,					//有日历提示信息

		isCalStart:    !0,					//标记为开始时间

		dateInfoClass: "date-info-start",	//开始时间icon样式

		range:		   {mindate: new Date(), maxdate: "2014-12-31"},	//限制范围（当天——2020-12-31）

		count: 		   2,					//日历个数

		monthStep:	   1					//切换上下月日历步长

});

//返程时间实例

var oBackCal = new Calendar({

    id: "#<%= lv_date_r.ClientID%>",			//触发显示日历元素ID

		isPopup:       !0,					//弹出式日历

		isPrevBtn:     !0,					//显示上月按钮

		isNextBtn:     !0,					//显示下月按钮

		isCloseBtn:    !0,					//显示关闭按钮

		isHoliday:     !0,					//节假日特殊显示

		isHolidayTips: !0,					//显示节假日1~3天/后1~3天信息

		isDateInfo:    !0,					//显示日期信息

		isMessage: 	   !0,					//有日历提示信息

		isCalEnd:      !0,					//标记为结束时间

		dateInfoClass: "date-info-end",		//结束时间icon样式

		range:		   {mindate: new Date(), maxdate: "2020-12-31"},	//限制范围（当天——2020-12-31）

		count: 		   2,					//日历个数

		monthStep:	   1					//切换上下月日历步长

});

//日期点击函数

function dateClick(obj) {

	switch(this.triggerNode.id) {

		case "lv_date_y":

			oBackCal.startDate = this.startDate = obj["data-date"];
			
			oBackCal.render(this.startDate);

			this.render(this.startDate);

			oBackCal.focus();
		
			break;

		case "lv_date_r":

			oDepCal.endDate = this.endDate = obj["data-date"];

			oDepCal.render(this.startDate);

			this.render(this.endDate);

			break;

	}

}

//日期点击自定义事件

oDepCal.on("dateClick", dateClick);

oBackCal.on("dateClick", dateClick);

//为返程实例添加className, 主要为了样式需要

oBackCal.container.className = "cal-end";

//开启返程实例的鼠标移动范围

oBackCal.showRange();

//开启键盘事件

oDepCal.CalStart = oBackCal.CalStart = oDepCal;

oDepCal.CalEnd   = oBackCal.CalEnd   = oBackCal;

oDepCal.keyup();

oBackCal.keyup();

//默认出发时间和返程时间

/*

oDepCal.startDate = oBackCal.startDate = _CAL.formatStrDate(oDepDate);

oDepCal.endDate   = oBackCal.endDate   = _CAL.formatStrDate(oBackDate);

oDepCal.setDateInfo(_CAL.formatStrDate(oDepDate));

oBackCal.setDateInfo(_CAL.formatStrDate(oBackDate));

*/

//简单验证

var oDepInput  = _CAL.$("#lv_date_y"),

	oBackInput = _CAL.$("#lv_date_r")




_CAL.on(_CAL.$("#J_search_form"), "submit", function(e) {

	//阻止默认事件

	var	aMessage   = ["请选择出发日期", "请选择返程日期", "日期格式为：yyyy-mm-dd", "返程日期不能早于出发日期，请重新选择"],

		iDepValue  = oDepInput.value.replace(/-|\//g, ""),

		iBackValue = oBackInput.value.replace(/-|\//g, "");

		

	if(iBackValue < iDepValue ) {

		oBackCal.showMessage(aMessage[3])	

	}

	else {

		$("#J_search_form").submit();

	}	
	_CAL.halt(e)

});