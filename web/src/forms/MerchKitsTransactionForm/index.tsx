import { FormikHelpers } from 'formik';
import { useCallback, useMemo } from 'react';

import { FormikWithBackup } from '../components/FormikWithBackup';

import { useConfiguration } from '@/context/Configuration';
import { useGetMerchKitById } from '@/queries/merchKits/useGetMerchKitById';
import { useGetMerchKitSizes } from '@/queries/merchKits/useGetMerchKitSizes';

import { MerchKitsTransactionFormContent } from './components/MerchKitsTransactionFormContent';
import { convertValuesToRequest } from './components/MerchKitsTransactionFormContent/utils/convertValuesToRequest';
import { getInitialMerchSizes } from './components/MerchKitsTransactionFormContent/utils/getInitialMerchSizeAmounts';
import {
	FORM_NAME,
	INITIAL_MERCH_KIT_AMOUNT,
	INITIAL_MERCH_STORAGE_ID,
	TransactionTypes,
} from './constants';
import { merchKitValidationSchema } from './schema';
import {
	MerchKitsTransactionFormProps,
	MerchKitsTransactionFormValues,
} from './types';

export function MerchKitsTransactionForm({
	handleClose,
	handleSubmit,
	kitId,
}: MerchKitsTransactionFormProps) {
	const {
		configurableValues: { merchStorages },
	} = useConfiguration();

	const defaultWarehouseId =
		Number(merchStorages[0].id) || INITIAL_MERCH_STORAGE_ID;

	const { data: kitData, isLoading, isFetching } = useGetMerchKitById(kitId);

	const { data: initialSizeData } = useGetMerchKitSizes(
		kitId,
		defaultWarehouseId,
		{
			enabled: !!defaultWarehouseId,
		}
	);

	const initialFormData: MerchKitsTransactionFormValues = useMemo(() => {
		const clothingItem = kitData?.compounds?.find(
			(compound) => compound.isClothing
		);
		const sizeCounts =
			clothingItem && initialSizeData?.itemSizes
				? getInitialMerchSizes(initialSizeData.itemSizes, clothingItem)
				: [];

		return {
			kitId,
			amount: INITIAL_MERCH_KIT_AMOUNT,
			transactionType: TransactionTypes.WriteOff.id,
			warehouseId: defaultWarehouseId,
			notes: '',
			warehouseRecipientId: null,
			userRecipientId: null,
			sizeCounts,
			recipient: null,
		};
	}, [kitId, defaultWarehouseId, kitData, initialSizeData]);

	const handleFormSubmit = useCallback(
		async (
			values: MerchKitsTransactionFormValues,
			{ setSubmitting }: FormikHelpers<MerchKitsTransactionFormValues>
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

	return (
		<FormikWithBackup
			enableReinitialize
			initialValues={initialFormData}
			validationSchema={merchKitValidationSchema}
			onSubmit={handleFormSubmit}
			onClose={handleClose}
			validateOnBlur={false}
			name={FORM_NAME}
			id={kitId}
		>
			{(formikProps) => (
				<MerchKitsTransactionFormContent
					{...formikProps}
					{...{
						kitId,
						handleClose,
						kitData,
						isKitLoading: isLoading || isFetching,
					}}
				/>
			)}
		</FormikWithBackup>
	);
}
