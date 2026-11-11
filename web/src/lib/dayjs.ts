/* eslint-disable import/no-default-export */

import dayjs from 'dayjs';
import 'dayjs/locale/en-gb';
import arraySupport from 'dayjs/plugin/arraySupport';
import customParseFormat from 'dayjs/plugin/customParseFormat';
import isBetween from 'dayjs/plugin/isBetween';
import isoWeek from 'dayjs/plugin/isoWeek';
import isSameOrAfter from 'dayjs/plugin/isSameOrAfter';
import isSameOrBefore from 'dayjs/plugin/isSameOrBefore';
import localizedFormat from 'dayjs/plugin/localizedFormat';
import objectSupport from 'dayjs/plugin/objectSupport';
import timezone from 'dayjs/plugin/timezone';
import toObject from 'dayjs/plugin/toObject';
import updateLocale from 'dayjs/plugin/updateLocale';
import utc from 'dayjs/plugin/utc';
import weekOfYear from 'dayjs/plugin/weekOfYear';

dayjs.locale('en-gb');

dayjs.extend(utc);
dayjs.extend(timezone);
dayjs.extend(toObject);
dayjs.extend(objectSupport);
dayjs.extend(arraySupport);
dayjs.extend(isSameOrAfter);
dayjs.extend(isSameOrBefore);
dayjs.extend(isBetween);
dayjs.extend(weekOfYear);
dayjs.extend(localizedFormat);
dayjs.extend(isoWeek);
dayjs.extend(updateLocale);
dayjs.extend(customParseFormat);

dayjs.updateLocale('en-gb', {
	weekStart: 1,
});

export default dayjs;
export { isDayjs } from 'dayjs';
export type { ConfigType, Dayjs } from 'dayjs';
