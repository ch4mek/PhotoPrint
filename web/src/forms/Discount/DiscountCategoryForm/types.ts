import { FormikHelpers } from 'formik';

import {
	DiscountCategoryFormValues,
	DiscountCategoryMutationData,
} from '@/types/discountCategories';

export interface DiscountCategoryFormProps {
	handleClose: VoidFunction;
	handleSubmit: (values: DiscountCategoryMutationData) => Promise<void>;
	categoryId?: number | string;
}

export interface HandleAddNewTagProps {
	values: DiscountCategoryFormValues;
	setFieldValue: FormikHelpers<DiscountCategoryFormValues>['setFieldValue'];
	setFieldError: FormikHelpers<DiscountCategoryFormValues>['setFieldError'];
	setFieldTouched: FormikHelpers<DiscountCategoryFormValues>['setFieldTouched'];
}
