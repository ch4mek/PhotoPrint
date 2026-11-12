import{NAVIGATION_STEP as r}from"../constants/calendar.js";import"dayjs";function t(t,n){var o="Prev"===n?-1:1;return t.add(o*r.MONTH,"month")}export{t as getNavigatedDate};
