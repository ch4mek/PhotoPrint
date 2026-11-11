import { Autoselect } from '@mms/mms-ui-library/components/Autoselects';
import { DatePicker } from '@mms/mms-ui-library/components/CustomDatePicker';
import { PopUp } from '@mms/mms-ui-library/components/PopUp';
import { RadioButton } from '@mms/mms-ui-library/components/RadioButton';
import { Typography } from '@mms/mms-ui-library/components/Typography';
import { FormikHelpers } from 'formik';
import { memo, useCallback, useMemo, useState } from 'react';

import { DAY_OFF_LABLE, HOLIDAYS_POPUP_TITLE } from '../HolidaysGrid/constants';
import { getDayjsDate } from '../HolidaysGrid/utils';

import { PopUpTitle } from '@/components/PopUpTitle';
import {
	getAddingPopUpTitle,
	getEditingPopUpTitle,
} from '@/constants/activityStatus';
import {
	MAX_DATE_DAY,
	MAX_DATE_MONTH,
	MAX_SELECT_YEAR,
	MIN_DATE_DAY,
	MIN_DATE_MONTH,
	MIN_SELECT_YEAR,
	YYYY_MM_DD_DAYJS_DATE_FORMAT,
} from '@/constants/date';
import {
	getIsAlreadyExistMessage,
	getIsAlreadyExistWithTryAgainMessage,
} from '@/constants/errors';
import { TextField } from '@/fields/Text';
import { FormikWithBackup } from '@/forms/components/FormikWithBackup';
import { getDateFormat } from '@/helpers/getDateFormat';
import dayjs, { Dayjs } from '@/lib/dayjs';
import {
	CalendarExclusions,
	CalendarExclusionsParams,
	GetCalendarExclusionsItem,
} from '@/queries/calendar/types';
import { CalendaryCountry } from '@/types/configuration';

import {
	DATE_TYPE_RADIO_BUTTONS_NAME,
	DEFAULT_HOLIDAY_VALUES,
	HOLIDAY_FORM_FIELDS,
	NEW_WORKDAY_DATE_TEXT,
	THE_DATE_AND_WORKDAY_DATE_TEXT,
	THE_DATE_TEXT,
	FORM_NAME,
} from './constants';
import { getValidationSchema } from './schema';
import {
	HolidayFieldsWrapper,
	RadioButtonsWrapper,
	RadioSectionWrapper,
} from './styles';
import { HolidayFormValues } from './types';

function HolidayFormInner({
	handleClose,
	holiday,
	countries,
	onSubmit,
}: {
	handleClose: VoidFunction;
	holiday: GetCalendarExclusionsItem | undefined;
	countries: CalendaryCountry[];
	onSubmit: (values: CalendarExclusionsParams) => Promise<object>;
}) {
	const isEditingForm = !!holiday;
	const [isHolidayTypeMode, setHolidayTypeMode] = useState(
		isEditingForm ? holiday.dateType === CalendarExclusions.Holiday : true
	);

	const validationSchema = useMemo(
		() => getValidationSchema(isHolidayTypeMode),
		[isHolidayTypeMode]
	);

	const initialValues: HolidayFormValues = useMemo(() => {
		if (!holiday) {
			return DEFAULT_HOLIDAY_VALUES;
		}

		const country = countries.find((item) => item.id === holiday.countryId);

		return {
			countryId: country?.name || null,
			dateType: holiday.dateType,
			date: getDayjsDate(holiday.date),
			newDate: getDayjsDate(holiday.newDate),
			name: holiday.name,
		};
	}, [holiday]);

	const handleSubmit = useCallback(
		async (
			values: HolidayFormValues,
			{ setSubmitting }: FormikHelpers<HolidayFormValues>
		) => {
			const country = countries.find((item) => item.name === values.countryId);

			const isSubmited = await onSubmit({
				id: holiday?.id || null,
				countryId: country?.id,
				dateType: values.dateType,
				date: values.date
					? getDateFormat(values.date, YYYY_MM_DD_DAYJS_DATE_FORMAT)
					: null,
				newDate: values.newDate
					? getDateFormat(values.newDate, YYYY_MM_DD_DAYJS_DATE_FORMAT)
					: null,
				name: values.name?.trim() || null,
			} as CalendarExclusionsParams);

			setSubmitting(false);

			if (isSubmited) {
				handleClose();
			}
		},
		[holiday, countries]
	);

	const [minDate, maxDate] = useMemo(
		() => [
			dayjs([MIN_SELECT_YEAR, MIN_DATE_MONTH, MIN_DATE_DAY]),
			dayjs([MAX_SELECT_YEAR, MAX_DATE_MONTH, MAX_DATE_DAY]),
		],
		[]
	);

	return (
		<FormikWithBackup
			enableReinitialize
			initialValues={initialValues}
			validationSchema={validationSchema}
			onSubmit={handleSubmit}
			onClose={handleClose}
			validateOnBlur={false}
			name={FORM_NAME}
			id={holiday?.id}
		>
			{({
				values,
				setFieldValue,
				submitForm,
				setFieldTouched,
				errors,
				touched,
				setFieldError,
				handleOutsideClose,
			}) => {
				const handleOnChangeRegisteredValue =
					(field: string) => (value: unknown) => {
						const registeredLocationValue = value as CalendaryCountry;

						setFieldValue(
							field,
							value ? registeredLocationValue.name : null,
							true
						);
						setFieldTouched(field, true);
					};

				const handleChangeField = (field: string, newValue: string) => {
					setFieldValue(field, newValue);
					setFieldTouched(field);
				};

				const resetField = (field: string, resetValue?: string | null) => {
					setFieldTouched(field, false);

					if (resetValue !== undefined) {
						setFieldValue(field, resetValue);
					}

					setFieldError(field, undefined);
				};

				const handleDateType = (field: string, newValue: string) => () => {
					setHolidayTypeMode(newValue === CalendarExclusions.Holiday);

					setFieldValue(field, newValue);

					resetField(HOLIDAY_FORM_FIELDS.countryId.name);
					resetField(HOLIDAY_FORM_FIELDS.date.name, null);
					resetField(HOLIDAY_FORM_FIELDS.name.name, '');
					resetField(HOLIDAY_FORM_FIELDS.newDate.name, null);
				};

				const handleDateChange =
					(field: string) => (newValue: Dayjs | null) => {
						setFieldValue(field, newValue);

						const isNeedValidateBothDates =
							isEditingForm && values.date && values.newDate;

						if (isNeedValidateBothDates) {
							setFieldTouched(HOLIDAY_FORM_FIELDS.date.name);
							setFieldTouched(HOLIDAY_FORM_FIELDS.newDate.name);
						} else {
							setFieldTouched(field);
						}
					};

				const handlePositiveControl = () => {
					submitForm().catch((error) => {
						if (
							error.response.data.Message ===
							getIsAlreadyExistWithTryAgainMessage(THE_DATE_TEXT)
						) {
							setFieldError(
								HOLIDAY_FORM_FIELDS.date.name,
								getIsAlreadyExistMessage(THE_DATE_TEXT)
							);
						}

						if (
							error.response.data.Message ===
							getIsAlreadyExistWithTryAgainMessage(NEW_WORKDAY_DATE_TEXT)
						) {
							setFieldError(
								HOLIDAY_FORM_FIELDS.newDate.name,
								getIsAlreadyExistMessage(NEW_WORKDAY_DATE_TEXT)
							);
						}

						if (
							error.response.data.Message ===
							getIsAlreadyExistWithTryAgainMessage(
								THE_DATE_AND_WORKDAY_DATE_TEXT
							)
						) {
							setFieldError(
								HOLIDAY_FORM_FIELDS.newDate.name,
								getIsAlreadyExistMessage(NEW_WORKDAY_DATE_TEXT)
							);
							setFieldError(
								HOLIDAY_FORM_FIELDS.date.name,
								getIsAlreadyExistMessage(THE_DATE_TEXT)
							);
						}
					});
				};

				return (
					<PopUp
						title={
							<PopUpTitle
								headerText={
									isEditingForm
										? getEditingPopUpTitle(HOLIDAYS_POPUP_TITLE)
										: getAddingPopUpTitle(HOLIDAYS_POPUP_TITLE)
								}
							/>
						}
						onClose={handleClose}
						onShadowClick={handleOutsideClose}
						type="form"
						controls={{
							positiveControl: {
								onClick: handlePositiveControl,
							},
							negativeControl: { onClick: handleClose },
						}}
						hasPaddings
					>
						<HolidayFieldsWrapper>
							<Autoselect
								label={HOLIDAY_FORM_FIELDS.countryId.label}
								value={values.countryId}
								onChange={handleOnChangeRegisteredValue(
									HOLIDAY_FORM_FIELDS.countryId.name
								)}
								valueField="name"
								error={Boolean(errors.countryId && touched.countryId)}
								errorText={errors.countryId}
								options={countries}
								isRequired
							/>
							<RadioSectionWrapper>
								<Typography variant="s-400">
									{HOLIDAY_FORM_FIELDS.dateType.label}
								</Typography>
								<RadioButtonsWrapper>
									<RadioButton
										onChange={handleDateType(
											HOLIDAY_FORM_FIELDS.dateType.name,
											CalendarExclusions.Holiday
										)}
										checked={values.dateType === CalendarExclusions.Holiday}
										forAttribute={CalendarExclusions.Holiday}
										label={CalendarExclusions.Holiday}
										disabled={isEditingForm}
										name={DATE_TYPE_RADIO_BUTTONS_NAME}
									/>
									<RadioButton
										onChange={handleDateType(
											HOLIDAY_FORM_FIELDS.dateType.name,
											CalendarExclusions.DayOff
										)}
										checked={values.dateType === CalendarExclusions.DayOff}
										forAttribute={CalendarExclusions.DayOff}
										label={DAY_OFF_LABLE}
										disabled={isEditingForm}
										name={DATE_TYPE_RADIO_BUTTONS_NAME}
									/>
								</RadioButtonsWrapper>
							</RadioSectionWrapper>
							{isHolidayTypeMode && (
								<TextField
									name={HOLIDAY_FORM_FIELDS.name.name}
									size="fullWidth"
									label={HOLIDAY_FORM_FIELDS.name.label}
									error={Boolean(errors.name && touched.name)}
									errorText={errors.name}
									onChange={handleChangeField}
									isRequired
								/>
							)}
							<DatePicker
								innerLabel={HOLIDAY_FORM_FIELDS.date.label}
								value={dayjs(values.date)}
								onChange={handleDateChange(HOLIDAY_FORM_FIELDS.date.name)}
								size="fullWidth"
								errorText={
									touched.date && errors.date ? String(errors.date) : undefined
								}
								minDate={minDate}
								maxDate={maxDate}
								isRequired
							/>
							{!isHolidayTypeMode && (
								<DatePicker
									innerLabel={HOLIDAY_FORM_FIELDS.newDate.label}
									value={dayjs(values.newDate)}
									onChange={handleDateChange(HOLIDAY_FORM_FIELDS.newDate.name)}
									size="fullWidth"
									errorText={
										touched.newDate && errors.newDate
											? String(errors.newDate)
											: undefined
									}
									minDate={minDate}
									maxDate={maxDate}
									isRequired
								/>
							)}
						</HolidayFieldsWrapper>
					</PopUp>
				);
			}}
		</FormikWithBackup>
	);
}

export const HolidayForm = memo(HolidayFormInner);
