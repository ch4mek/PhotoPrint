import { MainFormField, NULLABLE_FIELDS } from '../constants';

export const checkNullableField = (value: string) =>
	NULLABLE_FIELDS.includes(value as MainFormField);
