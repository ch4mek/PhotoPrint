import { FormikErrors, FormikTouched } from 'formik';

import { ProfileFormValues } from '../types';

export interface ProfileFieldsInnerProps {
	values: ProfileFormValues;
	errors: FormikErrors<ProfileFormValues>;
	touched: FormikTouched<ProfileFormValues>;
	setFieldValue: (fieldName: string, fieldValue: string | null) => void;
	setFieldTouched: (
		field: string,
		isTouched?: boolean | undefined,
		shouldValidate?: boolean | undefined
	) => Promise<void | FormikErrors<ProfileFormValues>>;
	validateField: (field: string) => Promise<string | undefined> | Promise<void>;
}
