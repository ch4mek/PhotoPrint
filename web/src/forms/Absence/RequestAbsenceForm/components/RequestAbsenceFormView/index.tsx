import { Autoselect } from '@mms/mms-ui-library/components/Autoselects';
import { CurrencyInput } from '@mms/mms-ui-library/components/CurrencyInput';
import { DatePicker } from '@mms/mms-ui-library/components/CustomDatePicker';
import { DatePickerRange } from '@mms/mms-ui-library/components/CustomDatePickerRange';
import { PopUp } from '@mms/mms-ui-library/components/PopUp';
import { useCallback, useEffect } from 'react';

import {
	fieldsNames,
	MAX_LENGTH_HOURS,
	REQUEST_POPUP_TITLE,
} from '../../constants';
import { useAbsenceCounts } from '../../hooks/useAbsenceCounts';
import { useFormHandlers } from '../../hooks/useRequestFormValues';

import { AttachFilesGroupWithTypes } from '@/components/AttachFilesGroupWithTypes';
import { CountValueItem } from '@/components/CountValueItem';
import { PositiveActionButton } from '@/components/PositiveActionButton';
import { ABSENCE_AVAILABLE, ABSENCE_BALANCE } from '@/constants/absence';
import {
	DATE_RANGE_IS_INVALID,
	REQUEST_ABSENCE_ISSUE,
} from '@/constants/errors';
import { TextArea } from '@/fields/TextArea';
import { getCountLabelWithUnit } from '@/helpers/absences';
import { checkIfOptionsEqualById } from '@/helpers/checkIfOptionEqualToValue';
import { extractKeyById } from '@/helpers/extractKey';
import dayjs from '@/lib/dayjs';
import { TrackingUnitType } from '@/types/absences';

import {
	ON_DISABLED_TOOLTIP_MESSAGE,
	requestFieldData,
	TEXTAREA_MIN_ROWS,
} from './constants';
import {
	CountValuesWrapper,
	DatePickerWrapper,
	FormWrapper,
	HoursInputWrapper,
	MilitaryDatePicker,
	RequestFieldGroupWrapper,
} from './styles';
import { RequestAbsenceFormViewProps } from './types';

export function RequestAbsenceFormView({
	formikProps,
	selectedAbsence,
	absences,
	absenceNamesOptions,
	holidays,
	isDisableForm,
	isShowLoader,
	popUpStylesConfig,
	documentHandlers,
	selectedDays,
	userId,
	handleClose,
	getRequestSelectedDays,
	setSelectedAbsence,
}: RequestAbsenceFormViewProps) {
	const {
		values,
		errors,
		touched,
		submitForm,
		setFieldError,
		handleOutsideClose,
	} = formikProps;

	const {
		handleAbsenceNameChange,
		handleDateChange,
		handleChangeDateRange,
		handleHoursFieldValueChange,
		handleAttachedFileChange,
		handleAttachedFileRemove,
	} = useFormHandlers({
		selectedAbsence,
		userId,
		absences,
		documentHandlers,
		selectedDays,
		setSelectedAbsence,
		getRequestSelectedDays,
	});

	const absenceCounts = useAbsenceCounts({
		selectedAbsence,
		hoursValue: values.hours,
		selectedDays,
	});

	const {
		selectedAbsenceUnitCount,
		selectedCount,
		remainingDays,
		isShowAvailable,
		isZeroDaysSelected,
	} = absenceCounts;

	const handlePositiveControl = useCallback(() => {
		submitForm().catch((error) => {
			const responseMessage = error?.response?.data?.Message;

			if (responseMessage === REQUEST_ABSENCE_ISSUE.dateRange) {
				setFieldError(fieldsNames.dateFrom, DATE_RANGE_IS_INVALID);
				setFieldError(fieldsNames.dateTo, DATE_RANGE_IS_INVALID);
			}
		});
	}, [submitForm, setFieldError]);

	useEffect(() => {
		if (values.absenceName && absences) {
			const matchingAbsence = absences.find((item) => {
				const itemId = item.inquiryTemplateId ?? item.agreementTemplateId;

				return itemId === values.absenceName?.id;
			});

			if (matchingAbsence && matchingAbsence !== selectedAbsence) {
				setSelectedAbsence(matchingAbsence);
			}
		}
	}, [values.absenceName, absences]);

	useEffect(() => {
		if (values.dateFrom && values.dateTo) {
			getRequestSelectedDays({
				dateFrom: values.dateFrom,
				dateTo: values.dateTo,
				templateId: selectedAbsence?.inquiryTemplateId || '',
				userId: userId as number,
			});
		}
	}, [
		values.dateFrom,
		values.dateTo,
		selectedAbsence?.inquiryTemplateId,
		userId,
		getRequestSelectedDays,
	]);

	useEffect(() => {
		if (values.absenceName && absences) {
			const matchingAbsence = absences.find((item) => {
				const itemId = item.inquiryTemplateId ?? item.agreementTemplateId;

				return itemId === values.absenceName?.id;
			});

			if (matchingAbsence && matchingAbsence !== selectedAbsence) {
				setSelectedAbsence(matchingAbsence);
			}
		}
	}, [values.absenceName, absences]);

	return (
		<PopUp
			type="form"
			showLoader={isShowLoader}
			title={REQUEST_POPUP_TITLE}
			onClose={handleClose}
			onShadowClick={handleOutsideClose}
			controls={{
				positiveControl: {
					customControl: (
						<PositiveActionButton
							disabled={isDisableForm}
							tooltipText={ON_DISABLED_TOOLTIP_MESSAGE}
							onClick={isShowLoader ? undefined : handlePositiveControl}
						/>
					),
				},
				negativeControl: { onClick: handleClose },
			}}
			stylesConfig={popUpStylesConfig}
		>
			<FormWrapper>
				<RequestFieldGroupWrapper>
					<Autoselect
						{...requestFieldData.absenceName}
						value={values.absenceName}
						onChange={handleAbsenceNameChange}
						options={absenceNamesOptions}
						error={Boolean(errors.absenceName) && Boolean(touched.absenceName)}
						disabled={isDisableForm}
						errorText={errors.absenceName}
						getKey={extractKeyById}
						checkIfOptionEqualToValue={checkIfOptionsEqualById}
						shouldBeClosedOnOutsideScroll={false}
						disableClearable
						isRequired
					/>
				</RequestFieldGroupWrapper>

				<RequestFieldGroupWrapper>
					{selectedAbsence?.trackingUnitType === TrackingUnitType.Hours ? (
						<MilitaryDatePicker>
							<DatePickerWrapper>
								<DatePicker
									{...requestFieldData.date}
									value={values.dateFrom ? dayjs(values.dateFrom) : null}
									onChange={handleDateChange}
									disabled={isDisableForm}
									size="fullWidth"
									calendarPosition="bottom-end"
									calendarProps={{
										holidays,
									}}
									errorText={
										(touched.dateFrom && errors.dateFrom) ||
										(touched.dateTo && errors.dateTo)
											? String(errors.dateFrom || errors.dateTo)
											: ''
									}
									isRequired
								/>
							</DatePickerWrapper>

							<HoursInputWrapper>
								<CurrencyInput
									{...requestFieldData.hours}
									onValueChange={handleHoursFieldValueChange}
									value={isZeroDaysSelected ? '0' : values.hours || ''}
									includeDecimalPart={false}
									maxLength={MAX_LENGTH_HOURS}
									size="fullWidth"
									disabled={isDisableForm}
									error={Boolean(errors.hours) && Boolean(touched.hours)}
									disabledCurrencyChange
									isRequired
								/>
							</HoursInputWrapper>
						</MilitaryDatePicker>
					) : (
						<DatePickerRange
							{...requestFieldData.dateRange}
							range={[
								values.dateFrom ? dayjs(values.dateFrom) : null,
								values.dateTo ? dayjs(values.dateTo) : null,
							]}
							onChange={handleChangeDateRange}
							size="fullWidth"
							disabled={isDisableForm}
							calendarProps={{
								holidays,
							}}
							error={
								(Boolean(errors.dateFrom) && Boolean(touched.dateFrom)) ||
								(Boolean(errors.dateTo) && Boolean(touched.dateTo))
							}
							errorText={
								errors.dateFrom || errors.dateTo
									? String(errors.dateFrom || errors.dateTo)
									: undefined
							}
							isRequired
						/>
					)}

					<CountValuesWrapper>
						<CountValueItem
							title={getCountLabelWithUnit(
								selectedAbsence?.trackingUnitType,
								isShowAvailable ? ABSENCE_AVAILABLE : ABSENCE_BALANCE
							)}
							count={selectedAbsenceUnitCount}
						/>
						<CountValueItem
							title={getCountLabelWithUnit(
								selectedAbsence?.trackingUnitType,
								'Selected'
							)}
							count={selectedCount || 0}
						/>
						{selectedAbsence?.isAgreement && (
							<CountValueItem
								title={getCountLabelWithUnit(
									selectedAbsence?.trackingUnitType,
									'Remaining'
								)}
								count={remainingDays}
							/>
						)}
					</CountValuesWrapper>
				</RequestFieldGroupWrapper>

				<TextArea
					{...requestFieldData.notes}
					size="fullWidth"
					disabled={isDisableForm}
					minRows={TEXTAREA_MIN_ROWS}
				/>

				<AttachFilesGroupWithTypes
					files={values.files}
					onAttachedFileChange={handleAttachedFileChange}
					onAttachedFileRemove={handleAttachedFileRemove}
					disabled={isDisableForm}
					isAttachMultiple
				/>
			</FormWrapper>
		</PopUp>
	);
}
