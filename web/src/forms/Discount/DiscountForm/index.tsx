import { FormikHelpers } from 'formik';
import { useCallback, useEffect, useMemo, useState } from 'react';

import { normalizeColorsToLightTheme } from '@/components/PlateToolbars/utils/normalizeColorsToLightTheme';
import { DEFAULT_MAXIMUM_ALLOWED_IMAGE_SIZE_KB } from '@/constants/allowedFileSizes';
import { imageValidExtensions } from '@/constants/fileExtensions';
import { formImageKeysExcludedFromBackup } from '@/constants/formBackupExcludedKeys';
import { initialEditorData } from '@/constants/plateEditor';
import { FormikWithBackup } from '@/forms/components/FormikWithBackup';
import {
	initialFormValues,
	FORM_NAME,
	fieldNames,
} from '@/forms/Discount/DiscountForm/constants';
import { validationSchema } from '@/forms/Discount/DiscountForm/schema';
import { DiscountFormProps } from '@/forms/Discount/DiscountForm/types';
import { convertFormValuesToRequestData } from '@/forms/Discount/DiscountForm/utils/convertFormValuesToRequestData';
import { convertResponseToInitialData } from '@/forms/Discount/DiscountForm/utils/convertResponseToInitialData';
import { generateImageDataUrl } from '@/helpers/generateImageDataUrl';
import { normalizeEditorData } from '@/helpers/normalizeEditorData';
import { useAttachDocument } from '@/hooks/useAttachDocument';
import { useEditorState } from '@/hooks/useEditorState';
import { useFormImageField } from '@/hooks/useFormImageField';
import { useGetDiscountById } from '@/queries/discounts/useGetDiscountById';
import { useGetFile } from '@/queries/useGetFile';
import { useUploadFile } from '@/queries/useUploadFile';
import { DiscountFormValues } from '@/types/discounts';

import { DiscountFormContent } from './components/DiscountFormContent';

export function DiscountForm({
	handleClose,
	handleSubmit,
	discountId,
}: DiscountFormProps) {
	const isEditMode = discountId != null;
	const [categoryId, setCategoryId] = useState<number | null>(null);

	const {
		data: discountData,
		isFetching,
		isLoading,
	} = useGetDiscountById(discountId, {
		enabled: isEditMode,
	});

	const imageId = discountData?.imageFileId;

	const {
		data: imageData,
		isFetching: isImageFetching,
		isLoading: isImageLoading,
	} = useGetFile(
		{
			fileId: imageId!,
		},
		{
			enabled: !!imageId,
		}
	);

	const { mutateAsync, isPending: isUploadImage } = useUploadFile();

	const { files, removeFile, handleFileChange } = useAttachDocument(
		imageValidExtensions,
		DEFAULT_MAXIMUM_ALLOWED_IMAGE_SIZE_KB
	);

	const parsedEditorData = useMemo(() => {
		if (discountData?.text) {
			try {
				return JSON.parse(discountData.text);
			} catch {
				return initialEditorData;
			}
		}

		return initialEditorData;
	}, [discountData]);

	const { editorData, setEditorData } = useEditorState(parsedEditorData);

	const initialFormData: DiscountFormValues = useMemo(() => {
		if (isEditMode && discountData) {
			setCategoryId(discountData.discountCategoryId);

			return convertResponseToInitialData(discountData);
		}

		return initialFormValues;
	}, [isEditMode, discountData]);

	const {
		selectedImage,
		setSelectedImage,
		handleImageChange,
		handleImageRemove,
	} = useFormImageField<DiscountFormValues>({
		fieldName: fieldNames.imageChangeMarker,
		handleFileChange,
		removeFile,
	});

	const handleFormSubmit = useCallback(
		async (
			values: DiscountFormValues,
			{ setSubmitting }: FormikHelpers<DiscountFormValues>
		) => {
			try {
				setSubmitting(true);
				const data = normalizeColorsToLightTheme(
					normalizeEditorData(editorData)
				);
				const text = JSON.stringify(data, null, 2);
				const file = files?.[0];

				if (file) {
					await mutateAsync(
						{ file },
						{
							onSuccess: async (res) => {
								if (!res?.data.fileId) {
									return;
								}

								await handleSubmit(
									convertFormValuesToRequestData(
										values,
										text,
										String(res?.data.fileId)
									)
								);
							},
						}
					);

					return;
				}

				const requestImageId =
					(!selectedImage && !file) || !imageId ? null : imageId;

				await handleSubmit(
					convertFormValuesToRequestData(values, text, requestImageId)
				);
			} finally {
				setSubmitting(false);
			}
		},
		[handleSubmit, editorData, selectedImage, files, imageId]
	);

	useEffect(() => {
		if (imageData) {
			setSelectedImage(generateImageDataUrl(imageData));
		}
	}, [imageData]);

	return (
		<FormikWithBackup
			initialValues={initialFormData}
			validationSchema={validationSchema}
			onSubmit={handleFormSubmit}
			onClose={handleClose}
			enableReinitialize
			validateOnBlur={false}
			name={FORM_NAME}
			id={discountId}
			valuesKeysExcludedFromBackup={formImageKeysExcludedFromBackup}
		>
			{(formikProps) => (
				<DiscountFormContent
					{...{
						formikProps,
						isUploadImage,
						isEditMode,
						categoryId,
						selectedImage,
						setCategoryId,
						setEditorData,
						discountData,
						parsedEditorData,
					}}
					isLoading={
						isLoading || isFetching || isImageFetching || isImageLoading
					}
					onImageChange={(e) => handleImageChange(e, formikProps.setFieldValue)}
					onImageRemove={() => handleImageRemove(formikProps.setFieldValue)}
					onClose={handleClose}
				/>
			)}
		</FormikWithBackup>
	);
}
