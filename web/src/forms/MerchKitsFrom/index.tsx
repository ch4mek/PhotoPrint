import { useTheme } from '@emotion/react';
import { PopUp } from '@mms/mms-ui-library/components/PopUp';
import { Typography } from '@mms/mms-ui-library/components/Typography';
import { FormikHelpers } from 'formik';
import { useCallback, useEffect, useMemo } from 'react';

import {
	MIN_AMOUNT_TEXT,
	TEXTAREA_MIN_ROWS,
	TOOLTIP_TEXT_CONTENT,
} from '../MerchElementForm/constants';
import { HintWrapper } from '../MerchElementForm/styles';

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
import { AmountInputsSection } from '@/pages/Merch/components/KitsTab/components/AmountInputsSection/AmountInputSection';
import { CompoundsSection } from '@/pages/Merch/components/KitsTab/components/CompoundSection/CompoundSection';
import { MerchStatuses } from '@/pages/Merch/constants';
import { useGetMerchKitById } from '@/queries/merchKits/useGetMerchKitById';
import { useGetFile } from '@/queries/useGetFile';
import { useUploadFile } from '@/queries/useUploadFile';
import { FilePurpose, ImageFileSize } from '@/types/file';

import { MerchKitStatusPopUp } from './components/MerchKitStatusPopUp';
import {
	FORM_NAME,
	getInitialFormValues,
	getMerchKitActionByStatus,
	merchKitActionConfig,
	MERCH_KITS_FIELD_TITLE,
	merchKitsFieldData,
} from './constants';
import { merchKitsValidationSchema } from './schema';
import { generateCircleInfoStylesConfig, MinAmountSection } from './styles';
import { MerchKitsFormProps, MerchKitsFormValues } from './types';
import { convertValuesToRequest } from './utils/convertValuesToRequest';
import { getPopUpStylesConfig } from './utils/getPopUpStylesConfig';

export function MerchKitsForm({
	handleClose,
	handleSubmit,
	handleUpdateSubmit,
	merchId,
}: MerchKitsFormProps) {
	const isEditMode = Boolean(merchId);
	const theme = useTheme();
	const [, windowHeight] = useResize();
	const {
		configurableValues: { merchStorages },
	} = useConfiguration();

	const { mutateAsync, isPending: isUploadImage } = useUploadFile();

	const { data, isLoading, isFetching } = useGetMerchKitById(merchId!, {
		enabled: isEditMode,
	});

	const {
		handleClose: handleCloseStatusPopUp,
		handleOpen: handleOpenStatusPopUp,
		isOpen: isStatusPopUpOpen,
	} = useShowModal();

	const isArchived = data?.status === MerchStatuses.archived;
	const kitAction = getMerchKitActionByStatus(data?.status);
	const kitActionConfig = kitAction ? merchKitActionConfig[kitAction] : null;

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

	const initialFormData: MerchKitsFormValues = useMemo(() => {
		if (isEditMode && data) {
			return {
				name: data.name,
				description: data.description ?? '',
				imageChangeMarker: data.photoFileId,
				photoFileId: data.photoFileId,
				compounds: data.compounds.map(({ itemId, amountInKit }) => ({
					amountInKit,
					itemId,
				})),
				minAmounts: data.minAmounts,
			};
		}

		return getInitialFormValues(merchStorages);
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
	} = useFormImageField<MerchKitsFormValues>({
		fieldName: merchKitsFieldData.imageChangeMarker.name,
		handleFileChange,
		removeFile,
	});

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
			values: MerchKitsFormValues,
			{ setSubmitting }: FormikHelpers<MerchKitsFormValues>
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

	const isShowLoader =
		isLoading || isFetching || isImageFetching || isImageLoading;

	const stylesConfig = useMemo(
		() => generateCircleInfoStylesConfig(theme),
		[theme]
	);

	const popUpTitle = isEditMode
		? getEditingPopUpTitle(MERCH_KITS_FIELD_TITLE)
		: getAddingPopUpTitle(MERCH_KITS_FIELD_TITLE);

	const showStatusPopUp = isStatusPopUpOpen && kitAction && merchId;

	return (
		<>
			<FormikWithBackup
				enableReinitialize
				initialValues={initialFormData}
				validationSchema={merchKitsValidationSchema}
				onSubmit={handleFormSubmit}
				onClose={handleClose}
				name={FORM_NAME}
				id={merchId}
				validateOnBlur={false}
				valuesKeysExcludedFromBackup={formImageKeysExcludedFromBackup}
			>
				{({ submitForm, isSubmitting, setFieldValue, handleOutsideClose }) => (
					<PopUp
						stylesConfig={getPopUpStylesConfig(theme, windowHeight)}
						type="form"
						showLoader={isSubmitting || isShowLoader || isUploadImage}
						title={popUpTitle}
						onClose={handleClose}
						onShadowClick={handleOutsideClose}
						controls={{
							positiveControl: {
								onClick: submitForm,
								disabled: isArchived,
							},
							negativeControl: { onClick: handleClose },
							extraControls: kitActionConfig
								? [
										{
											text: kitActionConfig.buttonText,
											view: 'light',
											onClick: handleOpenStatusPopUp,
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
							<TextField
								{...merchKitsFieldData.name}
								onChange={setFieldValue}
								isRequired
								disabled={isArchived}
							/>
							<TextArea
								size="fullWidth"
								minRows={TEXTAREA_MIN_ROWS}
								{...merchKitsFieldData.description}
								onChange={setFieldValue}
								disabled={isArchived}
							/>
							<CompoundsSection
								disabled={isArchived}
								kitCompounds={data?.compounds}
							/>
							<MinAmountSection>
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
								<AmountInputsSection
									merchStorages={merchStorages}
									disabled={isArchived}
								/>
							</MinAmountSection>
						</FormWrapper>
					</PopUp>
				)}
			</FormikWithBackup>

			{showStatusPopUp && (
				<MerchKitStatusPopUp
					kitId={merchId}
					action={kitAction}
					handleCloseForm={handleClose}
					handleCloseModal={handleCloseStatusPopUp}
				/>
			)}
		</>
	);
}
