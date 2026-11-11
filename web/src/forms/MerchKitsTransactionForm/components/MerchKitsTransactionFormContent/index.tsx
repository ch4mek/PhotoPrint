import { useTheme } from '@emotion/react';
import { Autoselect } from '@mms/mms-ui-library/components/Autoselects';
import { Counter } from '@mms/mms-ui-library/components/Counter';
import { Input } from '@mms/mms-ui-library/components/Input';
import { PopUp } from '@mms/mms-ui-library/components/PopUp';
import { useEffect, useMemo } from 'react';

import { getAddingPopUpTitle } from '@/constants/activityStatus';
import { FIELD_IS_REQUIRED } from '@/constants/errors';
import { useConfiguration } from '@/context/Configuration';
import { TextArea } from '@/fields/TextArea';
import { TEXTAREA_MIN_ROWS } from '@/forms/MerchElementForm/constants';
import {
	MAX_AMOUNT,
	merchStorageFieldData,
	MIN_AMOUNT,
	TRANSACTION_FIELD_TITLE,
	TransactionTypes,
} from '@/forms/MerchKitsTransactionForm/constants';
import {
	AvailableKits,
	KitAmountSection,
	KitName,
	ThreeInputsRowWrapper,
} from '@/forms/MerchKitsTransactionForm/styles';
import { FormWrapper } from '@/forms/styles';
import { checkIfOptionsEqualById } from '@/helpers/checkIfOptionEqualToValue';
import { extractKeyById } from '@/helpers/extractKey';
import { getRecipientOptions } from '@/helpers/getRecipientOptions';
import { useResize } from '@/hooks/useResize';
import { useGetMerchKitSizes } from '@/queries/merchKits/useGetMerchKitSizes';
import { useRecipientsLookup } from '@/queries/merchTransactions/useRecipientsLookup';
import { AutoselectOption } from '@/types/common';

import { CompoundsSection } from './components/CompundsSection';
import { ItemToTransactSection } from './components/ItemToTransactSection';
import { MerchKitsTransactionFormContentProps } from './types';
import { getCompounds } from './utils/getCompounds';
import { getInitialMerchSizes } from './utils/getInitialMerchSizeAmounts';
import { getPopUpStylesConfig } from './utils/getPopUpStylesConfig';
import { mergeCompoundsWithSizes } from './utils/mergeCompundsWithSizes';

export const MerchKitsTransactionFormContent = ({
	kitId,
	handleClose,
	kitData,
	isKitLoading,
	...formikProps
}: MerchKitsTransactionFormContentProps) => {
	const theme = useTheme();
	const [, windowHeight] = useResize();

	const {
		configurableValues: { merchStorages, merchSizes },
		persistentValues: { merchTransactionTypes },
	} = useConfiguration();

	const {
		submitForm,
		errors,
		isSubmitting,
		setFieldValue,
		setValues,
		values,
		setFieldTouched,
		touched,
		handleOutsideClose,
	} = formikProps;

	const { data: users, isLoading: isLoadingUsers } = useRecipientsLookup();

	const isShowLoader = isKitLoading || isLoadingUsers;

	const { data: sizeData, isLoading: isSizesLoading } = useGetMerchKitSizes(
		kitId,
		values.warehouseId,
		{
			enabled: !!values.warehouseId,
		}
	);

	useEffect(() => {
		if (!sizeData?.itemSizes || !kitData?.compounds) {
			return;
		}

		const clothingItem = kitData.compounds.find(
			(compound) => compound.isClothing
		);

		if (!clothingItem) {
			return;
		}

		const clothingSizes = getInitialMerchSizes(
			sizeData.itemSizes,
			clothingItem
		);

		setFieldValue('sizeCounts', clothingSizes);
	}, [sizeData, kitData, values.warehouseId, setFieldValue]);

	const isReciept = values.transactionType === TransactionTypes.Receipt.id;
	const isTransfer = values.transactionType === TransactionTypes.Transfer.id;

	const storageOptions = merchStorages.map(({ name }) => name);

	const transactionsOptions = merchTransactionTypes.map(
		({ name }) => TransactionTypes[name].value
	);

	const recipientOptions = useMemo(
		() => getRecipientOptions(isTransfer, merchStorages, users),
		[isTransfer, merchStorages, users]
	);

	const updatedCompounds = useMemo(
		() => getCompounds(kitData?.compounds || [], values.amount),
		[kitData?.compounds, values.amount]
	);

	const handleChangeStorage = (value: unknown) => {
		setFieldValue(
			'warehouseId',
			merchStorages.find(({ name }) => name === value)?.id
		);
		setFieldTouched('warehouseId', true);
	};

	const handleChangeAmount = (value: number) => {
		setFieldValue('amount', value);
	};

	const handleChangeType = (value: string | null) => {
		setValues({
			...values,
			transactionType: value?.split('-').join('') ?? '',
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

			return;
		}

		setFieldValue('warehouseRecipientId', isTransfer ? option.id : null);
		setFieldValue('userRecipientId', isTransfer ? null : option.id);
		setFieldValue('recipient', option);
	};

	const clothes = mergeCompoundsWithSizes(
		kitData?.compounds || [],
		sizeData?.itemSizes || [],
		merchSizes
	);

	const getStorageInputValue = (warehouseId: number) =>
		merchStorages.find(({ id }) => +id === warehouseId)?.name;

	return (
		<PopUp
			stylesConfig={getPopUpStylesConfig(theme, windowHeight)}
			type="form"
			showLoader={isSubmitting || isShowLoader || isSizesLoading}
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
						options={transactionsOptions}
						value={
							TransactionTypes[values.transactionType]
								? TransactionTypes[values.transactionType].value
								: ''
						}
						error={Boolean(errors.transactionType)}
						errorText={FIELD_IS_REQUIRED}
						onChange={handleChangeType}
						isRequired
					/>
					{!isReciept && (
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
				<KitAmountSection>
					<KitName>
						<Input
							size="fullWidth"
							value={kitData?.name || ''}
							disabled
							label="Kit name"
						/>
						{!isReciept && (
							<AvailableKits variant="s-400">
								Available: {sizeData?.availableAmountInStorage}
							</AvailableKits>
						)}
					</KitName>
					<Counter
						value={values.amount}
						onChange={handleChangeAmount}
						minValue={MIN_AMOUNT}
						maxValue={MAX_AMOUNT}
						editable
						{...merchStorageFieldData.kitAmount}
					/>
				</KitAmountSection>

				{clothes.length > 0 &&
					values.sizeCounts.length > 0 &&
					clothes.map(({ name, sizes, itemId }) => (
						<ItemToTransactSection
							key={itemId}
							itemName={name}
							sizes={sizes}
							isReciept={isReciept}
						/>
					))}

				<CompoundsSection compounds={updatedCompounds} />

				<TextArea
					size="fullWidth"
					minRows={TEXTAREA_MIN_ROWS}
					{...merchStorageFieldData.notes}
					onChange={setFieldValue}
				/>
			</FormWrapper>
		</PopUp>
	);
};
