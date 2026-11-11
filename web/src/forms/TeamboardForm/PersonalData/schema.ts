import * as yup from 'yup';

import { DATE_FORMAT } from '@/constants/date';
import {
	EMAILS_ARE_MATCH,
	FIELD_IS_REQUIRED,
	getPleaseEnterAValidFieldErrorMessage,
} from '@/constants/errors';
import { isChiefRole, isIntern, Roles } from '@/constants/roles';
import {
	dateType1Schema,
	emailSchema,
	emailType2Schema,
	notRequiredTextType4Schema,
	phoneNumberType2Schema,
	textType1Schema,
	textType1Dot2Schema,
} from '@/forms/schemas';
import { linkSchema } from '@/forms/schemas/types/linkType';
import { areCaseInsensitiveStringsEqual } from '@/helpers/areValuesEqual';
import dayjs from '@/lib/dayjs';

import { PersonalDataFormValues } from './types';
import { getErrorLabel } from './utils';

const personalEmailsNotEqualTest = {
	name: 'emails-not-equal',
	message: EMAILS_ARE_MATCH,
	test: (value: string | null | undefined, context: yup.TestContext) => {
		const { personalEmail, personalEmailSecond } = context.parent;

		if (context.path === 'personalEmail') {
			return !areCaseInsensitiveStringsEqual(value, personalEmailSecond);
		}

		return !areCaseInsensitiveStringsEqual(value, personalEmail);
	},
};

export const getValidationSchema = () =>
	yup.object().shape({
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
		position: yup.mixed().required(FIELD_IS_REQUIRED),
		department: yup
			.mixed()
			.test('is-ceo-or-cto', FIELD_IS_REQUIRED, (value, context) => {
				const positionId = (context.parent as PersonalDataFormValues).position
					?.id;

				if (isChiefRole(positionId as Roles)) {
					return true;
				}

				return value != null;
			}),
		group: yup.mixed(),
		corporateEmail: yup
			.string()
			.nullable()
			.when('position', (position, schema) => {
				const isInternPosition = isIntern(position?.id);

				return isInternPosition ? schema : emailSchema().nullable();
			}),
		personalEmail: yup
			.string()
			.when('position', (position) => {
				const isInternPosition = isIntern(position?.id);
				const baseSchema = emailType2Schema();

				return isInternPosition
					? baseSchema.required(FIELD_IS_REQUIRED)
					: baseSchema.optional();
			})
			.test(personalEmailsNotEqualTest),
		personalEmailSecond: emailType2Schema()
			.optional()
			.test(personalEmailsNotEqualTest),
		linkedIn: linkSchema(),
		personalPhone: phoneNumberType2Schema().optional(),
		englishLevel: yup.mixed().required(FIELD_IS_REQUIRED),
		telegram: notRequiredTextType4Schema(getErrorLabel('telegram')),
		militaryServiceStatus: yup.mixed(),
		graduateJobPlacement: yup.mixed(),
	});
