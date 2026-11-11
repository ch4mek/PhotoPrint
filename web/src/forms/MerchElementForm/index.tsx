import { useTheme } from '@emotion/react';
import { Checkbox } from '@mms/mms-ui-library/components/Checkbox';
import { PopUp } from '@mms/mms-ui-library/components/PopUp';
import { Typography } from '@mms/mms-ui-library/components/Typography';
import { FormikHelpers } from 'formik';
import { useCallback, useEffect, useMemo } from 'react';

import { ConfirmationDialog } from '../../components/ConfirmationDialog';

import { ImageUpload } from '@/components/ImageUpload';
import { InfoTooltipCircle } from '@/components/InfoTooltipCircle';
import {
	getAddingPopUpTitle,
	getEditingPopUpTitle,
} from '@/constants/activityStatus';
import { DEFAULT_MAXIMUM_ALLOWED_IMAGE_SIZE_KB } from '@/constants/allowedFileSizes';
import { imageValidExtensions } from '@/constants/fileExtensions';
import { formImageKeysExcludedFromBackup } from '@/constants/formBackupExcludedKeys';
import { useConfiguration } from '@/context/Configuration';
import { TextField } from '@/fields/Text';
import { TextArea } from '@/fields/TextArea';
import { FormikWithBackup } from '@/forms/components/FormikWithBackup';
import { FormWrapper } from '@/forms/styles';
import { generateImageDataUrl } from '@/helpers/generateImageDataUrl';
import { useAttachDocument } from '@/hooks/useAttachDocument';
import { useFormImageField } from '@/hooks/useFormImageField';
import { useResize } from '@/hooks/useResize';
import { useShowModal } from '@/pages/hooks/useShowModal';
import { SizesInputsSection } from '@/pages/Merch/components/ElementsTab/components/SizesInputsSection/SizesInputsSection';
import { useGetMerchElementById } from '@/queries/merchElements/useGetMerchElementById';
import { useGetFile } from '@/queries/useGetFile';
import { useUploadFile } from '@/queries/useUploadFile';
import { FilePurpose, ImageFileSize } from '@/types/file';

import {
	BUTTON_ACTIVATE_TEXT,
	BUTTON_ARCHIVE_TEXT,
	confirmationModalTextByStatus,
	getInitialFormValues,
	MERCH_ELEMENTS_FIELD_TITLE,
	merchElementsFieldData,
	MIN_AMOUNT_TEXT,
	successMessage,
	TEXTAREA_MIN_ROWS,
	TOOLTIP_TEXT_CONTENT,
	FORM_NAME,
} from './constants';
import { merchElementsValidationSchema } from './schema';
import {
	generateCircleInfoStylesConfig,
	HintWrapper,
	SizesSection,
	TwoInputsRowWrapper,
} from './styles';
import {
	MerchElementsFormProps,
	MerchElementsFormValues,
	MerchElementStatus,
} from './types';
import { convertValuesToRequest } from './utils/convertValuesToRequest';
import { getPopUpStylesConfig } from './utils/getPopupStyleConfig';
import { getValueByMerchElementStatus } from './utils/getValueByMerchElementStatus';

export function MerchElementForm({
	handleClose,
	handleSubmit,
	handleUpdateSubmit,
	handleUpdateStatus,
	merchId,
	isLoadingStatus,
}: MerchElementsFormProps) {
	const isEditMode = Boolean(merchId);
	const theme = useTheme();
	const [, windowHeight] = useResize();
	const {
		configurableValues: { merchSizes, merchStorages },
	} = useConfiguration();

	const {
		handleOpen: handleConfirmationOpen,
		handleClose: handleConfirmationClose,
		isOpen: isConfirmationOpen,
	} = useShowModal();

	const { mutateAsync, isPending: isUploadImage } = useUploadFile();

	const { data, isLoading, isFetching } = useGetMerchElementById(merchId!, {
		enabled: isEditMode,
	});

	const imageId = data?.photoFileId;

	const {
		data: imageData,
		isFetching: isImageFetching,
		isLoading: isImageLoading,
	} = useGetFile(
		{ fileId: imageId!, imageSize: ImageFileSize.Thumbnail },
		{
			enabled: !!imageId,
		}
	);

	const initialFormData: MerchElementsFormValues = useMemo(() => {
		if (isEditMode && data) {
			return {
				name: data.name,
				supplier: data.supplier ?? '',
				description: data.description ?? '',
				isClothing: data.isClothing,
				photoFileId: data.photoFileId,
				imageChangeMarker: data.photoFileId,
				sizeMinAmounts: data.sizeAmounts,
			};
		}

		return getInitialFormValues(merchStorages, merchSizes);
	}, [isEditMode, data]);

	const { files, removeFile, handleFileChange } = useAttachDocument(
		imageValidExtensions,
		DEFAULT_MAXIMUM_ALLOWED_IMAGE_SIZE_KB
	);

	const {
		selectedImage,
		setSelectedImage,
		handleImageChange,
		handleImageRemove,
	} = useFormImageField<MerchElementsFormValues>({
		fieldName: merchElementsFieldData.imageChangeMarker.name,
		handleFileChange,
		removeFile,
	});

	const isArchived = data?.status === MerchElementStatus.Archived;

	useEffect(() => {
		if (imageData) {
			setSelectedImage(generateImageDataUrl(imageData));
		}
	}, [imageData]);

	useEffect(() => {
		if (!imageId) {
			setSelectedImage(null);
		}
	}, [imageId]);

	const handleFormSubmit = useCallback(
		async (
			values: MerchElementsFormValues,
			{ setSubmitting }: FormikHelpers<MerchElementsFormValues>
		) => {
			try {
				setSubmitting(true);

				const file = files?.[0];

				if (file) {
					await mutateAsync(
						{ file, purpose: FilePurpose.Merch },
						{
							onSuccess: async (res) => {
								if (!res?.data.fileId) {
									return;
								}

								if (isEditMode) {
									await handleUpdateSubmit(
										convertValuesToRequest(
											values,
											String(res?.data.fileId),
											merchId
										)
									);
								} else {
									await handleSubmit(
										convertValuesToRequest(values, String(res?.data.fileId))
									);
								}
							},
						}
					);

					return;
				}

				const requestImageId =
					(!selectedImage && !file) || !imageId ? null : imageId;

				if (isEditMode) {
					await handleUpdateSubmit(
						convertValuesToRequest(values, requestImageId, merchId)
					);
				} else {
					await handleSubmit(convertValuesToRequest(values, requestImageId));
				}
			} finally {
				setSubmitting(false);
			}
		},
		[handleSubmit, handleUpdateSubmit, selectedImage, files, imageId]
	);

	const handleUpdateStatusSubmit = useCallback(
		(status: MerchElementStatus) => async () => {
			try {
				await handleUpdateStatus(
					{ status },
					getValueByMerchElementStatus({
						isArchived,
						isPartOfAvailableKit: data?.isPartOfAvailableKit,
						valueByStatus: successMessage,
					})
				);
			} finally {
				handleConfirmationClose();
			}
		},
		[handleUpdateStatus, handleConfirmationClose, data, isArchived]
	);

	const isShowLoader =
		isLoading || isFetching || isImageFetching || isImageLoading;

	const stylesConfig = useMemo(
		() => generateCircleInfoStylesConfig(theme),
		[theme]
	);

	const buttonText = isArchived ? BUTTON_ACTIVATE_TEXT : BUTTON_ARCHIVE_TEXT;
	const confirmationModalText = getValueByMerchElementStatus({
		isArchived,
		isPartOfAvailableKit: data?.isPartOfAvailableKit,
		valueByStatus: confirmationModalTextByStatus,
	});

	return (
		<>
			<FormikWithBackup
				enableReinitialize
				initialValues={initialFormData}
				validationSchema={merchElementsValidationSchema}
				onSubmit={handleFormSubmit}
				onClose={handleClose}
				validateOnBlur={false}
				name={FORM_NAME}
				id={merchId}
				valuesKeysExcludedFromBackup={formImageKeysExcludedFromBackup}
			>
				{({
					submitForm,
					isSubmitting,
					setFieldValue,
					values,
					setValues,
					handleOutsideClose,
				}) => {
					const handleCheckboxClick = (field: string) => () => {
						setValues({
							...values,
							sizeMinAmounts: initialFormData.sizeMinAmounts,
						});
						setFieldValue(field, !values.isClothing);
					};

					return (
						<PopUp
							stylesConfig={getPopUpStylesConfig(theme, windowHeight)}
							type="form"
							showLoader={isSubmitting || isShowLoader || isUploadImage}
							title={
								isEditMode
									? getEditingPopUpTitle(MERCH_ELEMENTS_FIELD_TITLE)
									: getAddingPopUpTitle(MERCH_ELEMENTS_FIELD_TITLE)
							}
							onClose={handleClose}
							onShadowClick={handleOutsideClose}
							controls={{
								positiveControl: { onClick: submitForm, disabled: isArchived },
								negativeControl: { onClick: handleClose },
								extraControls: isEditMode
									? [
											{
												view: 'light',
												text: buttonText,
												onClick: handleConfirmationOpen,
											},
									  ]
									: [],
							}}
						>
							<FormWrapper>
								<ImageUpload
									file={selectedImage}
									removeFile={() => handleImageRemove(setFieldValue)}
									handleFileChange={(e) => handleImageChange(e, setFieldValue)}
									disabled={isArchived}
								/>
								<TwoInputsRowWrapper>
									<TextField
										{...merchElementsFieldData.name}
										onChange={setFieldValue}
										isRequired
										disabled={isArchived}
									/>
									<TextField
										{...merchElementsFieldData.supplier}
										onChange={setFieldValue}
										disabled={isArchived}
									/>
								</TwoInputsRowWrapper>
								<TextArea
									size="fullWidth"
									minRows={TEXTAREA_MIN_ROWS}
									{...merchElementsFieldData.description}
									onChange={setFieldValue}
									disabled={isArchived}
								/>
								<Checkbox
									{...merchElementsFieldData.isClothing}
									disabled={isEditMode || isArchived}
									checked={values.isClothing ?? false}
									onClick={handleCheckboxClick(
										merchElementsFieldData.isClothing.name
									)}
								/>
								<SizesSection>
									<HintWrapper>
										<Typography
											color={theme.palette['text-black']}
											variant="s-600"
										>
											{MIN_AMOUNT_TEXT}
										</Typography>
										<InfoTooltipCircle
											isWithFallback
											tooltipContent={TOOLTIP_TEXT_CONTENT}
											stylesConfig={stylesConfig}
										/>
									</HintWrapper>
									<SizesInputsSection
										initialFormData={initialFormData}
										merchSizes={merchSizes}
										merchStorages={merchStorages}
										disabled={isArchived}
									/>
								</SizesSection>
							</FormWrapper>
						</PopUp>
					);
				}}
			</FormikWithBackup>
			{isConfirmationOpen && (
				<ConfirmationDialog
					{...confirmationModalText}
					handleClose={handleConfirmationClose}
					handleSubmit={handleUpdateStatusSubmit(
						isArchived
							? MerchElementStatus.Available
							: MerchElementStatus.Archived
					)}
					isLoading={isLoadingStatus}
				/>
			)}
		</>
	);
}
