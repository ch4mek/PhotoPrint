import * as yup from 'yup';

import { textType4Schema } from '@/forms/schemas';

import { groupMutationFormFieldsNames } from './constants';

export const validationSchema = yup.object({
	name: textType4Schema(groupMutationFormFieldsNames.name),
});
