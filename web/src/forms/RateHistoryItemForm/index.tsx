import { useCallback, useMemo } from 'react';

import {
	getAddingPopUpTitle,
	getEditingPopUpTitle,
} from '@/constants/activityStatus';
import {
	NotificationType,
	PROJECT_RATE_ADDED,
	PROJECT_RATE_UPDATED,
} from '@/constants/notifications';
import { useToast } from '@/context/Toast';
import { FormikWithBackup } from '@/forms/components/FormikWithBackup';
import { createToast } from '@/helpers/createToast';
import { createOnErrorHandler } from '@/helpers/requestErrorHandler';
import { sortCurrenciesByDisplayOrder } from '@/helpers/sortCurrencies';
import { useGetCurrenciesLookup } from '@/queries/currencies/useGetCurrenciesLookup';
import { useCreateProjectRate } from '@/queries/rates/useCreateProjectRate';
import { useUpdateProjectRate } from '@/queries/rates/useUpdateProjectRate';

import { RateHistoryItemFormContent } from './components/RateHistoryItemFormContent';
import { FORM_NAME, RATE_HISTORY_ITEM_ENTITY_NAME } from './constants';
import { validationSchema } from './schema';
import {
	RateHistoryItemFormProps,
	RateHistoryItemFormValues,
	ValidatedRateHistoryItemFormValues,
} from './types';
import {
	getInitialFormValues,
	mapCurrenciesToFormOptions,
	mapRateHistoryItemFormValuesToCreateParams,
	mapRateHistoryItemFormValuesToUpdateParams,
} from './utils';

export function RateHistoryItemForm({
	onClose,
	userId,
	projectId,
	rateItem,
}: RateHistoryItemFormProps) {
	const toast = useToast();
	const isEditMode = rateItem?.id != null;

	const { data: currencies = [], isLoading: isCurrenciesLoading } =
		useGetCurrenciesLookup();

	const currencyOptions = useMemo(
		() => mapCurrenciesToFormOptions(sortCurrenciesByDisplayOrder(currencies)),
		[currencies]
	);

	const initialValues = useMemo(
		() => getInitialFormValues(rateItem, currencyOptions),
		[rateItem, currencyOptions]
	);

	const { mutateAsync: createProjectRate, isPending: isCreating } =
		useCreateProjectRate({
			onSuccess: () => {
				toast.open(createToast(NotificationType.SUCCESS, PROJECT_RATE_ADDED));
				onClose();
			},
			onError: createOnErrorHandler(toast),
		});

	const { mutateAsync: updateProjectRate, isPending: isUpdating } =
		useUpdateProjectRate({
			onSuccess: () => {
				toast.open(createToast(NotificationType.SUCCESS, PROJECT_RATE_UPDATED));
				onClose();
			},
			onError: createOnErrorHandler(toast),
		});

	const handleSubmit = useCallback(
		async (values: RateHistoryItemFormValues) => {
			const validatedValues = values as ValidatedRateHistoryItemFormValues;

			if (rateItem?.id != null) {
				await updateProjectRate(
					mapRateHistoryItemFormValuesToUpdateParams(
						validatedValues,
						rateItem.id
					)
				);

				return;
			}

			await createProjectRate(
				mapRateHistoryItemFormValuesToCreateParams(
					validatedValues,
					userId,
					projectId
				)
			);
		},
		[rateItem?.id, updateProjectRate, createProjectRate, userId, projectId]
	);

	const popUpTitle = useMemo(
		() =>
			isEditMode
				? getEditingPopUpTitle(RATE_HISTORY_ITEM_ENTITY_NAME)
				: getAddingPopUpTitle(RATE_HISTORY_ITEM_ENTITY_NAME),
		[isEditMode]
	);

	const isLoading = isCreating || isUpdating || isCurrenciesLoading;

	return (
		<FormikWithBackup
			enableReinitialize
			onSubmit={handleSubmit}
			validateOnBlur={false}
			validateOnMount={false}
			name={FORM_NAME}
			id={rateItem?.id}
			{...{ validationSchema, initialValues, onClose }}
		>
			{({ handleOutsideClose }) => (
				<RateHistoryItemFormContent
					{...{
						onClose,
						popUpTitle,
						isLoading,
						handleOutsideClose,
						currencyOptions,
					}}
				/>
			)}
		</FormikWithBackup>
	);
}
