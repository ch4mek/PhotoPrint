import { Autoselect } from '@mms/mms-ui-library/components/Autoselects';
import { CurrencyInput } from '@mms/mms-ui-library/components/CurrencyInput';
import { MonthYearPicker } from '@mms/mms-ui-library/components/MonthYearPicker';
import { PopUp } from '@mms/mms-ui-library/components/PopUp';
import { useFormikContext } from 'formik';

import { rateHistoryItemFormFields } from '../../constants';
import { usePopupStylesConfig } from '../../hooks/usePopupStylesConfig';
import { RateHistoryItemFormFieldsRow } from '../../styles';
import {
	RateHistoryItemFormField,
	RateHistoryItemFormValues,
} from '../../types';

import { MAX_PICKER_DATE, MIN_PICKER_DATE } from '@/constants/date';
import { zIndexes } from '@/constants/zIndexes';
import { FormWrapper } from '@/forms/styles';
import { checkIfOptionsEqualById } from '@/helpers/checkIfOptionEqualToValue';
import { extractKeyById } from '@/helpers/extractKey';
import dayjs, { Dayjs } from '@/lib/dayjs';

import { RateHistoryItemFormContentProps } from './type';

export function RateHistoryItemFormContent({
	onClose,
	popUpTitle,
	isLoading,
	handleOutsideClose,
	currencyOptions,
}: RateHistoryItemFormContentProps) {
	const popUpStylesConfig = usePopupStylesConfig();
	const {
		values,
		errors,
		touched,
		initialValues,
		setFieldValue,
		setFieldTouched,
		submitForm,
	} = useFormikContext<RateHistoryItemFormValues>();

	const handleMonthYearChange =
		(fieldName: RateHistoryItemFormField) => (value: Dayjs) => {
			const normalizedValue = value.startOf('month');
			const initialDate = initialValues.date;
			const nextValue =
				initialDate != null && normalizedValue.isSame(initialDate, 'month')
					? initialDate
					: normalizedValue;

			setFieldValue(fieldName, nextValue, true);
			setFieldTouched(fieldName, true);
		};

	const handleRateChange =
		(fieldName: RateHistoryItemFormField) => (value: string) => {
			setFieldValue(fieldName, value, true);
			setFieldTouched(fieldName, true);
		};

	const handleCurrencyChange =
		(fieldName: RateHistoryItemFormField) => (value: unknown) => {
			setFieldValue(fieldName, value);
			setFieldTouched(fieldName, true);
		};

	const { date, rate, currency } = values;
	const {
		date: dateField,
		rate: rateField,
		currency: currencyField,
	} = rateHistoryItemFormFields;
	const isDateFilled = date != null;

	return (
		<PopUp
			type="form"
			title={popUpTitle}
			onClose={onClose}
			onShadowClick={handleOutsideClose}
			controls={{
				positiveControl: { onClick: submitForm },
				negativeControl: { onClick: onClose },
			}}
			zIndex={zIndexes.formPopup}
			showLoader={isLoading}
			stylesConfig={popUpStylesConfig}
		>
			<FormWrapper>
				<MonthYearPicker
					size="fullWidth"
					label={dateField.label}
					value={dayjs(date)}
					onChange={handleMonthYearChange(dateField.name)}
					error={!isDateFilled && Boolean(errors.date) && Boolean(touched.date)}
					errorText={
						!isDateFilled && touched.date && errors.date
							? String(errors.date)
							: ''
					}
					isRequired
					minDate={MIN_PICKER_DATE}
					maxDate={MAX_PICKER_DATE}
				/>
				<RateHistoryItemFormFieldsRow>
					<CurrencyInput
						size="fullWidth"
						label={rateField.label}
						value={rate}
						error={Boolean(errors.rate) && Boolean(touched.rate)}
						errorText={errors.rate}
						currency=""
						hideCurrencyChange
						onValueChange={handleRateChange(rateField.name)}
						isRequired
					/>
					<Autoselect
						label={currencyField.label}
						name={currencyField.name}
						value={currency}
						options={currencyOptions}
						onChange={handleCurrencyChange(currencyField.name)}
						error={Boolean(errors.currency) && touched.currency}
						errorText={errors.currency}
						getKey={extractKeyById}
						checkIfOptionEqualToValue={checkIfOptionsEqualById}
						valueField="name"
						isRequired
					/>
				</RateHistoryItemFormFieldsRow>
			</FormWrapper>
		</PopUp>
	);
}
