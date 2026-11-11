import dayjs from 'dayjs';
import * as yup from 'yup';

import { DATE_FORMAT } from '@/constants/date';
import { dateType1Schema } from '@/forms/schemas';
import { ddlType1Schema } from '@/forms/schemas/types/ddlType1';

export const getValidationSchema = (isAgreementMandatory: boolean) =>
	yup.object({
		absence: ddlType1Schema(),
		agreement: ddlType1Schema(isAgreementMandatory),
		effectiveFromDate: dateType1Schema(isAgreementMandatory).test(
			'is-future-date',
			'Future date is not allowed',
			(value) => {
				if (!value) {
					return true;
				}

				const date = dayjs(value, DATE_FORMAT, true);

				return date.isSameOrBefore(dayjs(), 'day');
			}
		),
	});
