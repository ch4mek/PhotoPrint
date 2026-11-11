import { Dayjs } from 'dayjs';
import { useFormikContext } from 'formik';
import {
	ChangeEvent,
	Dispatch,
	SetStateAction,
	useCallback,
	useEffect,
	useRef,
} from 'react';

import { fieldsNames } from '../constants';
import { InitialDataValues } from '../types';

import { getClampedHoursValue } from '@/forms/Absence/RequestAbsenceForm/utils/getClampedHoursValue';
import { RequestSelectedParam } from '@/queries/inquiries/types';
import { TrackingUnitType, UserAbsenceItem } from '@/types/absences';
import { SimpleConfigurableConfigurationItem } from '@/types/configuration';

import { useRequestDocuments } from './useRequestDocuments';

interface UseFormHandlersProps {
	selectedAbsence: UserAbsenceItem | null;
	userId: number | undefined;
	absences: UserAbsenceItem[] | undefined;
	documentHandlers: ReturnType<typeof useRequestDocuments>;
	selectedDays: number | null;
	getRequestSelectedDays: (params: RequestSelectedParam) => void;
	setSelectedAbsence: Dispatch<SetStateAction<UserAbsenceItem | null>>;
}
export const useFormHandlers = ({
	selectedAbsence,
	userId,
	absences,
	documentHandlers,
	selectedDays,
	setSelectedAbsence,
	getRequestSelectedDays,
}: UseFormHandlersProps) => {
	const {
		values,
		setFieldValue,
		setFieldTouched,
		setFieldError,
		validateForm,
		validateField,
	} = useFormikContext<InitialDataValues>();

	const { files, absenceName: absenceNameValue } = values;

	const previousTrackingUnitType = useRef<TrackingUnitType | undefined>(
		selectedAbsence?.trackingUnitType
	);

	const selectedDaysRef = useRef(selectedDays);
	selectedDaysRef.current = selectedDays;

	useEffect(() => {
		previousTrackingUnitType.current = selectedAbsence?.trackingUnitType;
	}, [selectedAbsence?.trackingUnitType]);

	useEffect(() => {
		const isHoursMode =
			selectedAbsence?.trackingUnitType === TrackingUnitType.Hours;

		if (!isHoursMode || !values.dateFrom) {
			return;
		}

		if (selectedDays === 0) {
			setFieldValue(fieldsNames.hours, '0', true);
			setFieldTouched(fieldsNames.hours, true, false);
		}
	}, [
		selectedDays,
		selectedAbsence,
		values.dateFrom,
		setFieldValue,
		setFieldTouched,
	]);

	const handleValueChange = useCallback(
		<T>({
			fieldName,
			value,
			isValidateValue = true,
			isFieldTouched = true,
			isResetError = false,
		}: {
			fieldName: string;
			value: T;
			isValidateValue?: boolean;
			isFieldTouched?: boolean;
			isResetError?: boolean;
		}) => {
			setFieldValue(fieldName, value, isValidateValue);
			setFieldTouched(fieldName, isFieldTouched);

			if (isResetError) {
				setFieldError(fieldName, undefined);
			}
		},
		[setFieldValue, setFieldTouched, setFieldError]
	);

	const handleCheckDatesRange = useCallback(
		(value: RequestSelectedParam) => {
			getRequestSelectedDays(value);
		},
		[getRequestSelectedDays]
	);

	const handleChangeDateRange = useCallback(
		async (value: [Dayjs | null, Dayjs | null]) => {
			const [dateFrom, dateTo] = value;
			const normalizedDateFrom = dateFrom?.startOf('day') ?? null;
			const normalizedDateTo = dateTo?.startOf('day') ?? null;
			const areValuesPresent = normalizedDateFrom && normalizedDateTo;

			await setFieldValue(fieldsNames.dateFrom, normalizedDateFrom, false);
			await setFieldValue(fieldsNames.dateTo, normalizedDateTo, false);
			await setFieldTouched(fieldsNames.dateFrom, true, false);
			await setFieldTouched(fieldsNames.dateTo, true, false);

			if (areValuesPresent && absenceNameValue) {
				handleCheckDatesRange({
					dateFrom: normalizedDateFrom,
					dateTo: normalizedDateTo,
					templateId: selectedAbsence?.inquiryTemplateId || '',
					userId: userId as number,
				});
			}

			validateForm();
		},
		[
			absenceNameValue,
			handleCheckDatesRange,
			selectedAbsence?.inquiryTemplateId,
			userId,
			setFieldValue,
			setFieldTouched,
			validateForm,
			handleValueChange,
		]
	);

	const handleDateChange = useCallback(
		async (newValue: Dayjs | null) => {
			if (newValue === null) {
				await setFieldValue(fieldsNames.dateFrom, null, true);
				await setFieldValue(fieldsNames.dateTo, null, true);

				return;
			}

			await setFieldValue(fieldsNames.dateFrom, newValue.toDate(), true);
			await setFieldValue(fieldsNames.dateTo, newValue.toDate(), true);
			setFieldTouched(fieldsNames.dateFrom, true);
			setFieldTouched(fieldsNames.dateTo, true);
			validateField(fieldsNames.dateFrom);
			validateField(fieldsNames.dateTo);
		},
		[validateField]
	);

	const handleAbsenceNameChange = useCallback(
		(value: SimpleConfigurableConfigurationItem<string | number> | null) => {
			const selectedAbsenceItem =
				absences?.find((item) => {
					const itemId = item.inquiryTemplateId ?? item.agreementTemplateId;

					return itemId === value?.id;
				}) || null;

			const selectedTrackingUnitType = selectedAbsenceItem?.trackingUnitType;

			handleValueChange({ fieldName: fieldsNames.absenceName, value });
			setSelectedAbsence(selectedAbsenceItem);

			if (selectedTrackingUnitType !== previousTrackingUnitType.current) {
				const resetFieldOptions = {
					isValidateValue: false,
					isFieldTouched: false,
					isResetError: true,
				};
				handleValueChange({
					fieldName: fieldsNames.dateFrom,
					value: null,
					...resetFieldOptions,
				});
				handleValueChange({
					fieldName: fieldsNames.dateTo,
					value: null,
					...resetFieldOptions,
				});
				handleValueChange({
					fieldName: fieldsNames.hours,
					value: '',
					...resetFieldOptions,
				});
				getRequestSelectedDays({
					dateFrom: null,
					dateTo: null,
					templateId: '',
					userId: userId as number,
				});
			}
		},
		[handleValueChange, getRequestSelectedDays, absences, userId]
	);

	const handleAttachedFileChange = useCallback(
		() => (e: ChangeEvent<HTMLInputElement>) => {
			const newFiles = documentHandlers.handleFileChange(e, files.length);

			if (newFiles) {
				setFieldValue('files', [...files, ...newFiles]);
			}

			return newFiles?.map(({ file }) => file);
		},
		[documentHandlers, files, setFieldValue]
	);

	const handleAttachedFileRemove = useCallback(
		(removedFileId: string | number) => () => {
			const updatedFiles = files.filter(
				({ fileId }) => fileId !== removedFileId
			);
			setFieldValue('files', updatedFiles);
			documentHandlers.handleFileRemove(removedFileId);
		},
		[files, setFieldValue, documentHandlers]
	);

	const handleHoursFieldValueChange = useCallback(
		(value: string) => {
			if (selectedDaysRef.current === 0) {
				handleValueChange({
					fieldName: fieldsNames.hours,
					value: '0',
					isValidateValue: false,
				});

				return;
			}

			if (value === '') {
				handleValueChange({
					fieldName: fieldsNames.hours,
					value: '',
					isValidateValue: true,
					isFieldTouched: true,
				});

				return;
			}

			handleValueChange({
				fieldName: fieldsNames.hours,
				value: getClampedHoursValue(Number(value)),
				isValidateValue: true,
				isFieldTouched: true,
				isResetError: false,
			});
		},
		[handleValueChange]
	);

	return {
		handleValueChange,
		handleChangeDateRange,
		handleAbsenceNameChange,
		handleDateChange,
		handleAttachedFileChange,
		handleAttachedFileRemove,
		handleHoursFieldValueChange,
	};
};
