import * as yup from 'yup';

import { FIELD_IS_REQUIRED } from '@/constants/errors';
import { isIntern, isNotChiefRole } from '@/constants/roles';
import {
	emailSchema,
	textType1Dot2Schema,
	textType1Schema,
} from '@/forms/schemas';
import { RoleType } from '@/types/configuration';

const schema = {
	firstName: textType1Schema('first name'),
	lastName: textType1Schema('last name'),
	firstNameRu: textType1Dot2Schema('first name (russian)'),
	lastNameRu: textType1Dot2Schema('last name (russian)'),
	email: yup.mixed().when('position', {
		is: (position: RoleType | null) => isIntern(position?.id),
		then: () => yup.string().strip().notRequired(),
		otherwise: () => emailSchema(),
	}),
	position: yup.mixed().required(FIELD_IS_REQUIRED).nullable(),
	currentLocation: textType1Schema('current location', true).nullable(),
};

export const validationSchema = yup.object().shape({
	...schema,
	location: yup.mixed().required(FIELD_IS_REQUIRED).nullable(),
	registeredLocationId: yup.mixed().required(FIELD_IS_REQUIRED).nullable(),
	department: yup.mixed().when('position', {
		is: (position: RoleType | null) => isNotChiefRole(position?.id),
		then: yup.mixed().required(FIELD_IS_REQUIRED).nullable(),
	}),
	group: yup.mixed().optional(),
});

export const validationSchemaSA = yup.object().shape({
	...schema,
});
