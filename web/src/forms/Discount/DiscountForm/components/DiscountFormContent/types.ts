import { ChangeEvent, Dispatch, SetStateAction } from 'react';

import { FormikWithBackupRenderProps } from '@/forms/components/FormikWithBackup/types';
import { Discount, DiscountFormValues } from '@/types/discounts';
import { CustomValue } from '@/types/plateEditor';

export interface DiscountFormContentProps {
	formikProps: FormikWithBackupRenderProps<DiscountFormValues>;
	categoryId: number | null;
	selectedImage: string | null;
	isEditMode: boolean;
	isLoading: boolean;
	isUploadImage: boolean;
	parsedEditorData: CustomValue;
	setCategoryId: Dispatch<SetStateAction<number | null>>;
	onClose: VoidFunction;
	onImageRemove: VoidFunction;
	onImageChange: (
		e: ChangeEvent<HTMLInputElement, Element>
	) => File[] | undefined;
	setEditorData: Dispatch<SetStateAction<CustomValue>>;
	discountData?: Discount;
}
