import { useTheme } from '@emotion/react';
import { Autoselect } from '@mms/mms-ui-library/components/Autoselects';
import { PopUp } from '@mms/mms-ui-library/components/PopUp';
import { FormikHelpers } from 'formik';
import { useCallback, useMemo } from 'react';

import { FormikWithBackup } from '../components/FormikWithBackup';
import { TEXTAREA_MIN_ROWS } from '../MerchElementForm/constants';

import { getAddingPopUpTitle } from '@/constants/activityStatus';
import { FIELD_IS_REQUIRED } from '@/constants/errors';
import { useConfiguration } from '@/context/Configuration';
import { TextArea } from '@/fields/TextArea';
import { FormWrapper } from '@/forms/styles';
import { checkIfOptionsEqualById } from '@/helpers/checkIfOptionEqualToValue';
import { extractKeyById } from '@/helpers/extractKey';
import { getRecipientOptions } from '@/helpers/getRecipientOptions';
import { useResize } from '@/hooks/useResize';
import { ItemToTransactSection } from '@/pages/Merch/components/StorageTab/components/ItemToTransactSection/ItemToTransactSection';
import { useGetMerchElementById } from '@/queries/merchElements/useGetMerchElementById';
import { useRecipientsLookup } from '@/queries/merchTransactions/useRecipientsLookup';
import { AutoselectOption } from '@/types/common';

import {
	FORM_NAME,
	INITIAL_MERCH_STORAGE_ID,
	merchStorageFieldData,
	TRANSACTION_FIELD_TITLE,
	transactionTypeOptions,
	transactionTypes,
} from './constants';
import { merchStorageValidationSchema } from './schema';
import { SizesSection, ThreeInputsRowWrapper } from './styles';
import {
	MerchStorageFormProps,
	MerchStorageFormValues,
	MerchTransactionOption,
} from './types';
import { convertValuesToRequest } from './utils/convertValuesToRequest';
import { getInitialMerchSizeAmounts } from './utils/getInitialMerchSizeMinAmounts';
import { getPopUpStylesConfig } from './utils/getPopUpStylesConfig';

export function MerchStorageForm({
	handleClose,
	handleSubmit,
	merchId,
	isAfterElementAdding = false,
}: MerchStorageFormProps) {
	const theme = useTheme();
	const [, windowHeight] = useResize();
	const {
		configurableValues: { merchSizes, merchStorages },
	} = useConfiguration();

	const { data, isLoading, isFetching } = useGetMerchElementById(merchId);

	const { data: users, isLoading: isLoadingUsers } = useRecipientsLookup();

	const initialFormData: MerchStorageFormValues = useMemo(
		() => ({
			itemId: Number(merchId),
			warehouseId: Number(merchStorages[0].id) || INITIAL_MERCH_STORAGE_ID,
			transactionType: isAfterElementAdding
				? transactionTypes.Receipt
				: transactionTypes.WriteOff,
			notes: '',
			warehouseRecipientId: null,
			userRecipientId: null,
			sizeCounts: getInitialMerchSizeAmounts(
				merchSizes,
				data?.isClothing ?? false
			),
			recipient: null,
		}),
		[data, merchStorages, merchId, isAfterElementAdding]
	);

	const handleFormSubmit = useCallback(
		async (
			values: MerchStorageFormValues,
			{ setSubmitting }: FormikHelpers<MerchStorageFormValues>
		) => {
			try {
				setSubmitting(true);
				await handleSubmit(convertValuesToRequest(values));
			} finally {
				setSubmitting(false);
			}
		},
		[handleSubmit]
	);

	const getStorageInputValue = (warehouseId: number) =>
		merchStorages.find(({ id }) => +id === warehouseId)?.name;

	const isShowLoader = isLoading || isFetching || isLoadingUsers;

	return (
		<FormikWithBackup
			enableReinitialize
			initialValues={initialFormData}
			validationSchema={merchStorageValidationSchema}
			onClose={handleClose}
			onSubmit={handleFormSubmit}
			validateOnBlur={false}
			name={FORM_NAME}
			id={merchId}
		>
			{({
				submitForm,
				errors,
				touched,
				isSubmitting,
				setFieldValue,
				setValues,
				values,
				setFieldTouched,
				handleOutsideClose,
			}) => {
				const { transactionType } = values;
				const isReceiptNotChosen =
					transactionType?.id !== transactionTypes.Receipt.id;
				const isTransfer = transactionType?.id === transactionTypes.Transfer.id;

				const storageOptions = merchStorages.map(({ name }) => name);

				const recipientOptions = getRecipientOptions(
					isTransfer,
					merchStorages,
					users
				);

				const handleChangeStorage = (value: unknown) => {
					setFieldValue(
						`warehouseId`,
						merchStorages.find(({ name }) => name === value)?.id
					);
					setFieldTouched(`warehouseId`, true);
				};

				const handleChangeType = async (
					option: MerchTransactionOption | null
				) => {
					setValues({
						...values,
						transactionType: option,
						warehouseRecipientId: null,
						userRecipientId: null,
						recipient: null,
					});
					setFieldTouched(`transactionType`, true);
				};

				const handleChangeRecipient = (option: AutoselectOption | null) => {
					if (!option) {
						setFieldValue('warehouseRecipientId', null);
						setFieldValue('userRecipientId', null);
						setFieldValue('recipient', null);
						setFieldTouched('recipient', true);

						return;
					}

					setFieldValue('warehouseRecipientId', isTransfer ? option.id : null);
					setFieldValue('userRecipientId', isTransfer ? null : option.id);
					setFieldValue('recipient', option);
					setFieldTouched('recipient', true);
				};

				return (
					<PopUp
						stylesConfig={getPopUpStylesConfig(theme, windowHeight)}
						type="form"
						showLoader={isSubmitting || isShowLoader}
						title={getAddingPopUpTitle(TRANSACTION_FIELD_TITLE)}
						onClose={handleClose}
						onShadowClick={handleOutsideClose}
						controls={{
							positiveControl: { onClick: submitForm },
							negativeControl: { onClick: handleClose },
						}}
					>
						<FormWrapper>
							<ThreeInputsRowWrapper>
								<Autoselect
									{...merchStorageFieldData.storage}
									options={storageOptions}
									value={getStorageInputValue(values.warehouseId)}
									onChange={handleChangeStorage}
									valueField="name"
									error={Boolean(errors.warehouseId)}
									errorText={errors.warehouseId && FIELD_IS_REQUIRED}
									isRequired
								/>
								<Autoselect
									{...merchStorageFieldData.transactionType}
									options={transactionTypeOptions}
									value={transactionType}
									error={Boolean(errors.transactionType)}
									errorText={FIELD_IS_REQUIRED}
									onChange={handleChangeType}
									disabled={isAfterElementAdding}
									getKey={extractKeyById}
									checkIfOptionEqualToValue={checkIfOptionsEqualById}
									valueField="name"
									isRequired
								/>
								{isReceiptNotChosen && (
									<Autoselect
										{...merchStorageFieldData.recipient}
										options={recipientOptions}
										value={values.recipient ?? null}
										onChange={handleChangeRecipient}
										error={Boolean(errors.recipient) && touched.recipient}
										errorText={FIELD_IS_REQUIRED}
										getKey={extractKeyById}
										checkIfOptionEqualToValue={checkIfOptionsEqualById}
										valueField="name"
										isRequired
									/>
								)}
							</ThreeInputsRowWrapper>
							<SizesSection>
								<ItemToTransactSection
									minAmountSizes={data?.sizeAmounts}
									isClothing={data?.isClothing}
									transactionType={transactionType?.id}
									itemName={data?.name}
								/>
							</SizesSection>
							<TextArea
								size="fullWidth"
								minRows={TEXTAREA_MIN_ROWS}
								{...merchStorageFieldData.notes}
								onChange={setFieldValue}
							/>
						</FormWrapper>
					</PopUp>
				);
			}}
		</FormikWithBackup>
	);
}
