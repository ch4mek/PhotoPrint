import dayjs from 'dayjs';
import * as yup from 'yup';

import { DATE_FORMAT } from '@/constants/date';
import { INVALID_DATE_RANGE } from '@/constants/notifications';
import { dateType1Schema } from '@/forms/schemas';
import { ddlType1Schema } from '@/forms/schemas/types/ddlType1';

export const validationSchema = yup.object().shape({
	approver: ddlType1Schema(),
	selectedUser: ddlType1Schema(),
	issueDate: dateType1Schema(),
	returnDate: dateType1Schema(false)
		.test('date-range', INVALID_DATE_RANGE, (returnDate, context) => {
			const { issueDate } = context.parent;

			if (!returnDate || !issueDate) {
				return true;
			}

			return dayjs(issueDate, DATE_FORMAT).isSameOrBefore(
				dayjs(returnDate, DATE_FORMAT),
				'day'
			);
		})
		.nullable(),
});
