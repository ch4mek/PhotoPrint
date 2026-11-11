import * as yup from 'yup';

import {
	dateType1Schema,
	emailSchema,
	emailType2Schema,
	phoneNumberType1Schema,
	textType1Dot2Schema,
	textType1Schema,
	textType4Schema,
} from '../../schemas';

import { DATE_FORMAT } from '@/constants/date';
import {
	EMAILS_ARE_MATCH,
	FIELD_IS_REQUIRED,
	getPleaseEnterAValidFieldErrorMessage,
} from '@/constants/errors';
import {
	isChiefRole,
	isHeadOfDepartment,
	isIntern,
	Roles,
} from '@/constants/roles';
import {
	dateType2FromDate,
	dateType2ToDate,
} from '@/forms/schemas/types/dateType2';
import { linkSchema } from '@/forms/schemas/types/linkType';
import { areCaseInsensitiveStringsEqual } from '@/helpers/areValuesEqual';
import dayjs from '@/lib/dayjs';

import { ADMIN_PANEL_USER_FORM_FIELDS } from './constants';
import { getErrorLabel } from './utils/getErrorLabel';

const whenInternPosition = (
	internSchema: yup.AnySchema,
	defaultSchema: yup.AnySchema
) =>
	yup.mixed().when(`${ADMIN_PANEL_USER_FORM_FIELDS.position.name}.id`, {
		is: (positionId: Roles) => isIntern(positionId),
		then: () => internSchema,
		otherwise: () => defaultSchema,
	});

export const validationSchema = yup.object().shape({
	lastName: textType1Schema(getErrorLabel('lastName')),
	firstName: textType1Schema(getErrorLabel('firstName')),
	lastNameRu: textType1Dot2Schema(getErrorLabel('lastNameRu'), true),
	firstNameRu: textType1Dot2Schema(getErrorLabel('firstNameRu'), true),
	patronymicRu: textType1Dot2Schema(getErrorLabel('patronymicRu'), true),
	dateOfBirth: dateType1Schema().test(
		'not-today-or-future',
		getPleaseEnterAValidFieldErrorMessage('date'),
		(value) => {
			if (!value) {
				return false;
			}
			const inputDate = dayjs(value, DATE_FORMAT).startOf('day');
			const today = dayjs().startOf('day');

			return inputDate.isBefore(today);
		}
	),
	militaryServiceStatus: yup.mixed(),

	personalPhone: phoneNumberType1Schema(false),
	telegram: textType4Schema(ADMIN_PANEL_USER_FORM_FIELDS.telegram.name, false),
	personalEmail: emailType2Schema()
		.test('intern-required', FIELD_IS_REQUIRED, (value, context) => {
			const position = context.parent.position?.id;

			if (isIntern(position)) {
				return Boolean(value);
			}

			return true;
		})
		.test('emails-not-equal', EMAILS_ARE_MATCH, (value, context) => {
			const { personalEmailSecond } = context.parent;

			return !areCaseInsensitiveStringsEqual(value, personalEmailSecond);
		}),
	personalEmailSecond: emailType2Schema().test(
		'emails-not-equal',
		EMAILS_ARE_MATCH,
		(value, context) => {
			const { personalEmail } = context.parent;

			return !areCaseInsensitiveStringsEqual(value, personalEmail);
		}
	),
	linkedIn: linkSchema(),

	position: yup.mixed().required(FIELD_IS_REQUIRED),
	department: yup
		.mixed()
		.test('department-required', FIELD_IS_REQUIRED, (value, context) => {
			const position = context.parent.position?.id;
			const { role, hasHRModuleClaim } = context.options.context ?? {};

			if (isChiefRole(position)) {
				return true;
			}

			if (isHeadOfDepartment(role) && !hasHRModuleClaim) {
				return true;
			}

			return Boolean(value);
		}),

	group: yup.mixed(),
	corporateEmail: whenInternPosition(
		yup.string().strip().notRequired(),
		emailSchema()
	),

	hireDate: dateType1Schema(),
	contractStartDate: whenInternPosition(
		dateType2FromDate(ADMIN_PANEL_USER_FORM_FIELDS.contractEndDate.name, true),
		dateType2FromDate(ADMIN_PANEL_USER_FORM_FIELDS.contractEndDate.name)
	),
	contractEndDate: whenInternPosition(
		dateType2ToDate(ADMIN_PANEL_USER_FORM_FIELDS.contractStartDate.name, true),
		dateType2ToDate(ADMIN_PANEL_USER_FORM_FIELDS.contractStartDate.name)
	),
	graduateJobPlacement: yup.mixed(),

	currentLocation: textType1Schema(
		getErrorLabel('currentLocation'),
		true
	).nullable(),
	officeLocation: yup.mixed().required(FIELD_IS_REQUIRED),
	registeredLocation: yup.mixed().required(FIELD_IS_REQUIRED),
});
