import { useTheme } from '@emotion/react';
import { Autoselect } from '@mms/mms-ui-library/components/Autoselects';
import { Button } from '@mms/mms-ui-library/components/Button';
import { CurrencyInput } from '@mms/mms-ui-library/components/CurrencyInput';
import { DatePicker } from '@mms/mms-ui-library/components/CustomDatePicker';
import { useQueryClient } from '@tanstack/react-query';
import { FormikHelpers } from 'formik';
import numeral from 'numeral';
import { memo, useEffect, useLayoutEffect, useMemo, useState } from 'react';

import {
	EquipmentTabs,
	popupFormKeys,
	StatusEquipment,
	STORAGE_OPTIONS,
} from '../constants';
import { FormDirtyTracker } from '../FormDirtyTracker';
import { FormContent } from '../styles';

import { Currency } from '@/constants/currency';
import {
	INITIAL_ORDER_BY_VALUE,
	NUMERAL_PRICE_FORMAT,
} from '@/constants/equipment';
import { DEFAULT_ROWS_PER_PAGE } from '@/constants/pagination';
import {
	ASSIGNMENTS_HISTORY_QUERY_KEY,
	EquipmentQueriesKeys,
} from '@/constants/queriesKeys';
import { managerRolesWithGroupManager } from '@/constants/roles';
import { DefaultSortFields, SortDirection } from '@/constants/sort';
import { useConfiguration } from '@/context/Configuration';
import { useEquipmentContext } from '@/context/EquipmentContext/Equipment';
import { useToast } from '@/context/Toast';
import { TextField } from '@/fields/Text';
import { TextArea } from '@/fields/TextArea';
import { FormikWithBackup } from '@/forms/components/FormikWithBackup';
import {
	checkIfOptionsEqualById,
	checkIfPrimitiveOptionsEqual,
} from '@/helpers/checkIfOptionEqualToValue';
import { extractKeyById, extractKeyFromPrimitive } from '@/helpers/extractKey';
import { findById } from '@/helpers/findById';
import { getTrimedString } from '@/helpers/getTrimedString';
import { updateGridItems } from '@/helpers/updateGridItems';
import { getFullName } from '@/helpers/utils';
import { usePageWithSearch } from '@/hooks/usePageWithSearch';
import dayjs, { Dayjs } from '@/lib/dayjs';
import { useCreateEquipment } from '@/queries/equipment';
import { useEquipmentUsersLookup } from '@/queries/equipment/useEquipmentUsersLookup';
import { useGetEquipmentById } from '@/queries/equipment/useGetEquipmentById';
import { useUpdateEquipment } from '@/queries/equipment/useUpdateEquipment';
import type {
	Approver,
	EquipmentGridRow,
	EquipmentItemRequest,
	EquipmentItemResponse,
} from '@/types/equipment';

import {
	MAIN_FORM_FIELDS,
	NOT_UNIQUE_SERIAL_NUMBER_ERROR,
	REPAIR_PLACE_REQUIRED_STATUSES,
	TEXT_AREA_MIN_ROWS,
	FORM_NAME,
	NOT_UNIQUE_INVENTORY_NUMBER_ERROR,
} from './constants';
import { getValidationSchema } from './schema';
import type { MainEquipmentFormProps, MainEquipmentFormValues } from './types';
import {
	convertEquipmentResponseToFormValues,
	convertStatusToRequestBrokenStatus,
	getStatusOptions,
} from './utils';

function MainEquipmentFormInner({
	setEquipmentId,
	id: equipmentId,
	setIsFormValid,
	onLoad,
	onClose,
	submitFormButtonRef,
	setActiveTab,
	setStatus,
}: MainEquipmentFormProps) {
	const theme = useTheme();
	const queryClient = useQueryClient();
	const toast = useToast();
	const { equipmentModel, setEquipmentModel } = useEquipmentContext();
	const { page, search } = usePageWithSearch();

	const [isSerialNumberUnique, setIsSerialNumberUnique] = useState(true);
	const [isInventoryNumberUnique, setIsInventoryNumberUnique] = useState(true);

	const isEditing = Boolean(equipmentId);

	const onSuccess = {
		onSuccess: (response: EquipmentItemResponse) => {
			const { id, status } = response;
			setEquipmentId(id);
			queryClient.invalidateQueries({
				queryKey: [EquipmentQueriesKeys.equipments],
			});
			queryClient.invalidateQueries({
				queryKey: [ASSIGNMENTS_HISTORY_QUERY_KEY],
			});

			onLoad(false);
			setStatus(status);

			if (!equipmentId) {
				setActiveTab(EquipmentTabs.USER);
			}
		},
	};

	const {
		persistentValues: {
			equipmentLocationTypes,
			equipmentAvailabilityTypes,
			equipmentBrokenStatuses,
		},
		configurableValues: { equipmentTypes },
	} = useConfiguration();

	const {
		data: equipmentResponse,
		isFetching: isFetchingPreData,
		refetch: refetchPreData,
	} = useGetEquipmentById(toast, equipmentId ?? '', {
		onSuccess: ({ status }) => {
			setStatus(status);
		},
	});

	const { mutate } = useCreateEquipment(toast, setIsSerialNumberUnique, {
		onSuccess: onSuccess.onSuccess,
		onError: () => {
			onLoad(false);
		},
	});

	const { mutate: updateMutation, isPending } = useUpdateEquipment(
		toast,
		setIsSerialNumberUnique,
		setIsInventoryNumberUnique,
		{
			onSuccess: (response) => {
				queryClient.setQueryData(
					[
						EquipmentQueriesKeys.equipments,
						page + 1,
						Number(DEFAULT_ROWS_PER_PAGE),
						SortDirection.ASC,
						INITIAL_ORDER_BY_VALUE,
						search,
						null,
						undefined,
						undefined,
						undefined,
						undefined,
						null,
						null,
					],
					(prev: EquipmentGridRow) => {
						updateGridItems(
							prev,
							response,
							DefaultSortFields.TYPE,
							SortDirection.ASC
						);
						queryClient.invalidateQueries({
							queryKey: [EquipmentQueriesKeys.equipments],
						});
						queryClient.invalidateQueries({
							queryKey: [ASSIGNMENTS_HISTORY_QUERY_KEY],
						});
					}
				);
				refetchPreData();
			},
			onError: () => {
				onLoad(false);
			},
		}
	);

	useEffect(() => {
		if (!isPending && equipmentResponse && equipmentTypes) {
			setEquipmentModel(
				convertEquipmentResponseToFormValues(equipmentResponse, equipmentTypes)
			);
		}
	}, [isPending, equipmentResponse, equipmentTypes]);

	const { data, isFetching: isFetchingUsers } = useEquipmentUsersLookup(
		managerRolesWithGroupManager
	);

	const approverOptions: Array<Approver> = (data || []).map(
		({ id, firstName, lastName }) => ({
			id,
			fullName: getFullName(firstName, lastName),
		})
	);
	const isEquipmentAssigned = Boolean(equipmentResponse?.activeHolder);

	const equipmentTypeOptions = equipmentTypes || [];
	const locationOptions = equipmentLocationTypes || [];
	const statusOptions = useMemo(
		() => getStatusOptions(equipmentAvailabilityTypes, isEquipmentAssigned),
		[isEquipmentAssigned, equipmentAvailabilityTypes]
	);

	useLayoutEffect(() => {
		onLoad(isFetchingPreData || isFetchingUsers);
	}, [isFetchingPreData, isFetchingUsers]);

	const handleSubmitForm = (
		values: MainEquipmentFormValues,
		{ validateForm }: FormikHelpers<MainEquipmentFormValues>
	) => {
		if (!validateForm(values)) {
			return;
		}

		const {
			purchasePrice,
			purchasePriceUsd,
			equipmentType,
			location,
			name,
			serialNumber,
			inventoryNumber,
			invoiceNumber,
			purchasePlace,
			approver: approverValue,
			status,
			jiraTask,
			repairPlace,
			storage,
		} = values;

		const request: EquipmentItemRequest = {
			...values,
			brokenStatus: convertStatusToRequestBrokenStatus(
				equipmentBrokenStatuses,
				status
			),
			purchasePrice: numeral(purchasePrice).value() ?? undefined,
			purchasePriceUsd: numeral(purchasePriceUsd).value() ?? undefined,
			equipmentTypeId: equipmentType?.id,
			location: (location as unknown as { id: string })?.id,
			approverId: approverValue?.id as number,
			name: name ? getTrimedString(name) : '',
			serialNumber: serialNumber ? getTrimedString(serialNumber) : '',
			inventoryNumber: inventoryNumber ? getTrimedString(inventoryNumber) : '',
			invoiceNumber: invoiceNumber ? getTrimedString(invoiceNumber) : '',
			purchasePlace: purchasePlace ? getTrimedString(purchasePlace) : '',
			jiraTask: jiraTask ? getTrimedString(jiraTask) : '',
			storage: storage ?? undefined,
			repairPlace: repairPlace || undefined,
		};
		onLoad(true);

		if (equipmentId) {
			updateMutation({ ...request, equipmentId }, onSuccess);
		} else {
			mutate({ ...request }, onSuccess);
		}
	};

	const initialValues: MainEquipmentFormValues = useMemo(() => {
		if (
			equipmentId &&
			equipmentResponse &&
			equipmentLocationTypes &&
			equipmentTypes
		) {
			const {
				approver,
				equipmentTypeId,
				location,
				guaranteeDate,
				purchaseDate,
				purchasePrice,
				purchasePriceUsd,
				status,
				activeHolder,
				jiraTask,
				storage,
				repairPlace,
				comment,
				...restProps
			} = equipmentResponse;

			return {
				...restProps,
				inventoryNumber: equipmentResponse.inventoryNumber || '',
				equipmentType: findById(equipmentTypes, equipmentTypeId) || null,
				status,
				purchasePrice: numeral(purchasePrice).format(NUMERAL_PRICE_FORMAT),
				purchasePriceUsd:
					numeral(purchasePriceUsd).format(NUMERAL_PRICE_FORMAT),
				purchasePriceUsdCurrency: Currency.USD,
				purchaseDate: dayjs(purchaseDate).startOf('day'),
				guaranteeDate: dayjs(guaranteeDate).startOf('day'),
				approver: approver
					? {
							id: approver.id,
							fullName: approver.fullName,
					  }
					: approver,
				location: findById(equipmentLocationTypes, location) || null,
				storage,
				activeHolder,
				jiraTask,
				repairPlace: repairPlace ?? '',
				comment: comment ?? '',
			};
		}

		return equipmentModel;
	}, [
		equipmentId,
		equipmentModel,
		equipmentLocationTypes,
		equipmentTypes,
		equipmentResponse,
	]);

	return (
		<FormikWithBackup
			validationSchema={getValidationSchema(isEditing)}
			validateOnMount={Boolean(equipmentModel.name)}
			onSubmit={handleSubmitForm}
			enableReinitialize
			validateOnChange
			validateOnBlur={false}
			name={FORM_NAME}
			id={equipmentId}
			{...{ initialValues, onClose }}
		>
			{({
				values,
				errors,
				touched,
				setFieldValue,
				setFieldTouched,
				validateField,
			}) => {
				const {
					equipmentType,
					status,
					location,
					approver,
					storage,
					purchaseDate,
					guaranteeDate,
					purchaseCurrency,
					purchasePrice,
					purchasePriceUsd,
					purchasePriceUsdCurrency,
				} = values;

				const handleInputChange = (fieldName: string, fieldValue: string) => {
					setFieldValue(fieldName, fieldValue, true);
					setFieldTouched(fieldName, true);

					if (fieldName === MAIN_FORM_FIELDS.serialNumber.name) {
						setIsSerialNumberUnique(true);
					} else if (fieldName === MAIN_FORM_FIELDS.inventoryNumber.name) {
						setIsInventoryNumberUnique(true);
					}
				};

				const handleAutoselectChange = (field: string) => (value: unknown) => {
					setFieldValue(field, value);
					setFieldTouched(field, true);
					validateField(field);
				};

				const handleStatusChange = (value: unknown) => {
					const statusField = MAIN_FORM_FIELDS.status.name;
					const isGoingFromRenovationToIssued =
						status === StatusEquipment.RENOVATION &&
						value === StatusEquipment.ISSUED;

					setFieldTouched(MAIN_FORM_FIELDS.repairPlace.name, false);

					if (statusField && isGoingFromRenovationToIssued) {
						setFieldValue(MAIN_FORM_FIELDS.repairPlace.name, '');
					}

					handleAutoselectChange(statusField)(value);
				};

				const handleInputCurrencyChange =
					(fieldName: string) => (value: string) => {
						setFieldValue(fieldName, value, true);
						setFieldTouched(fieldName, true);
					};

				const handleDateChange =
					(fieldName: string) => (value: Dayjs | null) => {
						setFieldValue(fieldName, value, true);
						setFieldTouched(fieldName, true);
					};

				const inventoryNumberHasError =
					isEditing &&
					(!isInventoryNumberUnique ||
						(errors.inventoryNumber && touched.inventoryNumber));

				let inventoryNumberErrorText: string | undefined;

				if (isEditing) {
					inventoryNumberErrorText = !isInventoryNumberUnique
						? NOT_UNIQUE_INVENTORY_NUMBER_ERROR
						: errors.inventoryNumber;
				}

				const isFormValid = Object.keys(errors).length === 0 && equipmentId;

				setIsFormValid(Boolean(isFormValid));

				return (
					<>
						<FormDirtyTracker formKey={popupFormKeys.main} />
						<FormContent>
							<TextField
								{...MAIN_FORM_FIELDS.equipmentName}
								onChange={handleInputChange}
								isRequired
							/>
							<Autoselect
								{...MAIN_FORM_FIELDS.equipmentType}
								value={equipmentType}
								options={equipmentTypeOptions}
								onChange={handleAutoselectChange(
									MAIN_FORM_FIELDS.equipmentType.name
								)}
								error={Boolean(errors.equipmentType) && touched.equipmentType}
								errorText={errors.equipmentType}
								getKey={extractKeyById}
								checkIfOptionEqualToValue={checkIfOptionsEqualById}
								isRequired
							/>
							<Autoselect
								{...MAIN_FORM_FIELDS.location}
								value={location}
								options={locationOptions}
								onChange={handleAutoselectChange(
									MAIN_FORM_FIELDS.location.name
								)}
								error={Boolean(errors.location) && touched.location}
								errorText={errors.location}
								getKey={extractKeyById}
								checkIfOptionEqualToValue={checkIfOptionsEqualById}
								isRequired
							/>

							<Autoselect
								{...MAIN_FORM_FIELDS.status}
								value={status}
								options={statusOptions}
								error={Boolean(errors.status) && touched.status}
								errorText={errors.status}
								isRequired
								onChange={handleStatusChange}
								getKey={extractKeyFromPrimitive}
								checkIfOptionEqualToValue={checkIfPrimitiveOptionsEqual}
							/>
							<TextField
								{...MAIN_FORM_FIELDS.invoiceNumber}
								onChange={handleInputChange}
								isRequired
							/>
							<TextField
								{...MAIN_FORM_FIELDS.serialNumber}
								onChange={handleInputChange}
								error={
									!isSerialNumberUnique ||
									(errors.serialNumber && touched.serialNumber)
										? true
										: undefined
								}
								errorText={
									!isSerialNumberUnique
										? NOT_UNIQUE_SERIAL_NUMBER_ERROR
										: errors.serialNumber
								}
								isRequired
							/>
							<TextField
								{...MAIN_FORM_FIELDS.inventoryNumber}
								onChange={handleInputChange}
								error={inventoryNumberHasError || undefined}
								errorText={inventoryNumberErrorText}
								isRequired={isEditing}
								disabled={!isEditing}
							/>

							<div>
								<DatePicker
									innerLabel={MAIN_FORM_FIELDS.purchaseDate.label}
									size="fullWidth"
									errorText={
										touched.purchaseDate && errors.purchaseDate
											? String(errors.purchaseDate)
											: ''
									}
									value={dayjs(purchaseDate)}
									onChange={handleDateChange(
										MAIN_FORM_FIELDS.purchaseDate.name
									)}
									isRequired
								/>
								<DatePicker
									innerLabel={MAIN_FORM_FIELDS.guaranteeDate.label}
									size="fullWidth"
									errorText={
										touched.guaranteeDate && errors.guaranteeDate
											? String(errors.guaranteeDate)
											: ''
									}
									value={dayjs(guaranteeDate)}
									onChange={handleDateChange(
										MAIN_FORM_FIELDS.guaranteeDate.name
									)}
									isRequired
								/>
								<Autoselect
									{...MAIN_FORM_FIELDS.approver}
									value={approver || null}
									options={approverOptions}
									onChange={handleAutoselectChange(
										MAIN_FORM_FIELDS.approver.name
									)}
									error={Boolean(errors.approver) && touched.approver}
									errorText={errors.approver}
									getKey={extractKeyById}
									checkIfOptionEqualToValue={checkIfOptionsEqualById}
									isRequired
								/>
								<Autoselect
									{...MAIN_FORM_FIELDS.storage}
									value={storage || null}
									options={STORAGE_OPTIONS}
									onChange={handleAutoselectChange(
										MAIN_FORM_FIELDS.storage.name
									)}
									error={Boolean(errors.storage) && touched.storage}
									errorText={errors.storage}
									getKey={extractKeyFromPrimitive}
									checkIfOptionEqualToValue={checkIfPrimitiveOptionsEqual}
									isRequired
								/>
							</div>
							<div>
								<CurrencyInput
									size="fullWidth"
									label={MAIN_FORM_FIELDS.purchasePrice.label}
									value={purchasePrice || ''}
									error={
										Boolean(errors.purchasePrice) &&
										Boolean(touched.purchasePrice)
									}
									errorText={errors.purchasePrice}
									currency={purchaseCurrency || ''}
									onValueChange={handleInputCurrencyChange(
										MAIN_FORM_FIELDS.purchasePrice.name
									)}
									onCurrencyChange={handleInputCurrencyChange(
										MAIN_FORM_FIELDS.purchasePrice.currency
									)}
									isRequired
								/>
								<CurrencyInput
									size="fullWidth"
									label={MAIN_FORM_FIELDS.purchasePriceUsd.label}
									value={purchasePriceUsd || ''}
									error={
										Boolean(errors.purchasePriceUsd) &&
										Boolean(touched.purchasePriceUsd)
									}
									errorText={errors.purchasePriceUsd}
									currency={purchasePriceUsdCurrency || ''}
									currenciesOptions={MAIN_FORM_FIELDS.purchasePriceUsd.options}
									disabledCurrencyChange
									dropdownLabelText={theme.palette['text-gray']}
									onValueChange={handleInputCurrencyChange(
										MAIN_FORM_FIELDS.purchasePriceUsd.name
									)}
									onCurrencyChange={handleInputCurrencyChange(
										MAIN_FORM_FIELDS.purchasePriceUsd.currency
									)}
									isRequired
								/>

								<TextField
									{...MAIN_FORM_FIELDS.purchasePlace}
									size="fullWidth"
									onChange={handleInputChange}
									isRequired
								/>
								<TextField
									{...MAIN_FORM_FIELDS.repairPlace}
									size="fullWidth"
									onChange={handleInputChange}
									errorText={errors.repairPlace}
									isRequired={REPAIR_PLACE_REQUIRED_STATUSES.includes(status)}
								/>
							</div>
							<TextArea
								{...MAIN_FORM_FIELDS.characteristics}
								size="fullWidth"
								minRows={TEXT_AREA_MIN_ROWS}
								onChange={handleInputChange}
								isRequired
							/>
							<TextField
								{...MAIN_FORM_FIELDS.jiraTask}
								error={Boolean(errors.jiraTask) && Boolean(touched.jiraTask)}
								size="fullWidth"
								onChange={handleInputChange}
								isRequired
							/>
							<TextArea
								{...MAIN_FORM_FIELDS.comment}
								size="fullWidth"
								minRows={TEXT_AREA_MIN_ROWS}
								onChange={handleInputChange}
							/>
							<Button
								type="submit"
								ref={submitFormButtonRef}
								style={{ display: 'none' }}
							>
								Submit
							</Button>
						</FormContent>
					</>
				);
			}}
		</FormikWithBackup>
	);
}

export const MainEquipmentForm = memo(MainEquipmentFormInner);
