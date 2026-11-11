import { FormikHelpers, FormikValues } from 'formik';
import { ChangeEvent, useCallback, useState } from 'react';

import { convertFileToBase64 } from '@/helpers/convertFileToBase64';

interface UseFormImageFieldParams<Values extends FormikValues> {
	fieldName: keyof Values & string;
	handleFileChange: (e: ChangeEvent<HTMLInputElement>) => File[] | undefined;
	removeFile: VoidFunction;
	getChangedFieldValue?: (file: File) => string | null;
	removedFieldValue?: string | null;
}

const getDefaultChangedFieldValue = (file: File) =>
	`${file.name}_${file.lastModified}`;

export function useFormImageField<Values extends FormikValues>({
	fieldName,
	handleFileChange,
	removeFile,
	getChangedFieldValue = getDefaultChangedFieldValue,
	removedFieldValue = null,
}: UseFormImageFieldParams<Values>) {
	const [selectedImage, setSelectedImage] = useState<string | null>(null);

	const handleImageChange = useCallback(
		(
			event: ChangeEvent<HTMLInputElement>,
			setFieldValue: FormikHelpers<Values>['setFieldValue']
		) => {
			const selectedFiles = handleFileChange(event);
			const selectedFile = selectedFiles?.[0];

			if (selectedFile) {
				convertFileToBase64(selectedFile).then((convertedFile) => {
					setSelectedImage(convertedFile);
				});
				setFieldValue(fieldName, getChangedFieldValue(selectedFile), false);
			}

			return selectedFiles;
		},
		[fieldName, handleFileChange, getChangedFieldValue]
	);

	const handleImageRemove = useCallback(
		(setFieldValue: FormikHelpers<Values>['setFieldValue']) => {
			removeFile();
			setSelectedImage(null);
			setFieldValue(fieldName, removedFieldValue, false);
		},
		[fieldName, removeFile, removedFieldValue]
	);

	return {
		selectedImage,
		setSelectedImage,
		handleImageChange,
		handleImageRemove,
	};
}
