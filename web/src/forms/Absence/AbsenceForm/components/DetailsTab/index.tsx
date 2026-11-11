import { Autoselect } from '@mms/mms-ui-library/components/Autoselects';
import { Checkbox } from '@mms/mms-ui-library/components/Checkbox';
import { TextArea as TextAreaField } from '@mms/mms-ui-library/components/TextArea';
import { spaces } from '@mms/mms-ui-library/theme';
import { Form } from 'formik';
import { ChangeEvent, useMemo } from 'react';

import { FieldWithDescription } from '../FieldWithDescription';

import { FormSection } from '@/components/FormSection';
import {
	absencesType,
	accrualsType,
	activitiesType,
	trackingUnitsType,
	transfersType,
} from '@/constants/absence';
import { useConfiguration } from '@/context/Configuration';
import { TextField } from '@/fields/Text';
import { FormWrapper } from '@/forms/styles';
import { extractKeyById } from '@/helpers/extractKey';
import { AbsenceDetailsFields } from '@/types/absences';

import {
	formSections,
	absenceDetailsFieldData,
	detailsCheckBoxesFieldData,
	wrapperStyles,
	MIN_COMMENT_COUNT_OF_ROWS,
	fieldNames,
} from './constants';
import { AutoSelectFiledsWrapper, CheckboxLabel } from './styles';
import { AutoSelectFieldData, DetailsTabProps } from './types';
import { convertDataToOptions } from './utils/convertDataToOptions';

const { details, limits } = formSections;
const {
	type,
	trackingUnitType,
	activityType,
	accrualType,
	transferType,
	comment,
} = absenceDetailsFieldData;

function DetailsTabInner({
	values,
	errors = {},
	touched = {},
	setFieldValue,
	setFieldTouched,
	validateField,
	absenceTypesOptions,
	activityTypesOptions,
	accrualTypeOptions,
	disableAutoselectFields,
}: DetailsTabProps) {
	const {
		persistentValues: { trackingUnitTypes, transferTypes },
	} = useConfiguration();

	const isVacationOrSickDaySelected =
		values.type?.name === absencesType.Vacation ||
		values.type?.name === absencesType.SickDay;

	const isDocumentsRequestSelected =
		values.type?.name === absencesType.Documents;

	const isOnceInYearSelected =
		values.accrualType?.name === accrualsType.OnceInYear;

	const allTrackingUnitOptions = convertDataToOptions(
		trackingUnitsType,
		trackingUnitTypes
	);

	const currentTrackingUnitTypesOptions = isVacationOrSickDaySelected
		? allTrackingUnitOptions.filter(
				(item) => item.name === trackingUnitsType.Days
		  )
		: allTrackingUnitOptions;

	const currentTransferTypeOptions = isOnceInYearSelected
		? convertDataToOptions(transfersType, transferTypes)
		: convertDataToOptions(transfersType, transferTypes).filter(
				(item) => item.name === transfersType.Transferred
		  );

	const autoSelectFiledData: AutoSelectFieldData = {
		firstPartFields: [
			{
				name: type.name,
				options: absenceTypesOptions,
				withWrapper: false,
			},
			...(isDocumentsRequestSelected
				? []
				: [
						{
							name: trackingUnitType.name,
							options: currentTrackingUnitTypesOptions,
							withWrapper: false,
						},
				  ]),
			...(isVacationOrSickDaySelected
				? [
						{
							name: accrualType.name,
							withWrapper: false,
							options: accrualTypeOptions,
						},
						{
							name: transferType.name,
							withWrapper: false,
							options: currentTransferTypeOptions,
						},
				  ]
				: []),
		],
		secondPartFields: isDocumentsRequestSelected
			? []
			: [
					{
						name: activityType.name,
						options: activityTypesOptions,
						withWrapper: true,
					},
			  ],
	};

	const updateField = (field: string, value: unknown, isTouched = true) => {
		const fieldPath = `details.${field}`;
		const isValueNotNull = value !== null;

		setFieldValue(fieldPath, value);
		setFieldTouched(fieldPath, isTouched);

		if (isValueNotNull) {
			validateField(fieldPath);
		}
	};

	const handleAutoselectChange =
		(field: AbsenceDetailsFields) => (value: unknown) => {
			updateField(field, value);

			if (field === type.name) {
				const newTypeName = (value as { name: string })?.name;
				const isVacationOrSickDay =
					newTypeName === absencesType.Vacation ||
					newTypeName === absencesType.SickDay;

				if (!isVacationOrSickDay) {
					updateField(accrualType.name, null, false);
					updateField(transferType.name, null, false);
				}

				const isDocumentsType = newTypeName === absencesType.Documents;
				const daysTrackingUnitOption =
					allTrackingUnitOptions.find(
						({ name }) => name === trackingUnitsType.Days
					) ?? null;

				if (isDocumentsType) {
					updateField(trackingUnitType.name, daysTrackingUnitOption, false);
					updateField(
						activityType.name,
						activityTypesOptions.find(
							({ name }) => name === activitiesType.WorkingPaid
						) ?? null
					);
				} else if (isVacationOrSickDay) {
					const currentTrackingUnit = values.trackingUnitType;

					if (currentTrackingUnit?.name === trackingUnitsType.Hours) {
						updateField(trackingUnitType.name, daysTrackingUnitOption, false);
					}
				}
			}

			if (field === accrualType.name) {
				const isOnceInYear =
					(value as { name: string })?.name === accrualsType.OnceInYear;

				if (!isOnceInYear) {
					const currentTransferType = values[transferType.name];

					if (currentTransferType?.toString() !== transfersType.Transferred) {
						updateField(transferType.name, null, false);
					}
				}
			}
		};

	const handleTextFieldChange = (field: string, value: string) => {
		updateField(field, value);
	};

	const handleTextAreaChange = (e: ChangeEvent<HTMLTextAreaElement>) => {
		updateField(comment.name, e.target.value);
	};

	const handleCheckBoxClick = (field: AbsenceDetailsFields) => () => {
		updateField(field, !values[field]);
	};

	const filteredCheckBoxes = useMemo(
		() =>
			detailsCheckBoxesFieldData.filter(
				({ name }) =>
					!isDocumentsRequestSelected ||
					(name !== fieldNames.isIncludeWeekends &&
						name !== fieldNames.isIncludeHolidays)
			),
		[isDocumentsRequestSelected]
	);

	return (
		<Form>
			<FormWrapper>
				<FormSection title={details}>
					<FormWrapper>
						<TextField
							{...absenceDetailsFieldData.name}
							onChange={handleTextFieldChange}
							isRequired
							value={values.name}
							errorText={errors.name}
							error={Boolean(errors.name && touched.name)}
						/>
						<AutoSelectFiledsWrapper
							isSingleColumn={isDocumentsRequestSelected}
						>
							{autoSelectFiledData.firstPartFields.map(({ name, options }) => (
								<Autoselect
									key={name}
									{...absenceDetailsFieldData[name]}
									isRequired
									disableClearable
									value={values[name]}
									onChange={handleAutoselectChange(name)}
									options={options || []}
									errorText={errors[name]}
									error={Boolean(errors[name] && touched[name])}
									getKey={extractKeyById}
									disabled={disableAutoselectFields}
								/>
							))}
						</AutoSelectFiledsWrapper>
						{autoSelectFiledData.secondPartFields.map(
							({ name, options, withWrapper }) => {
								const field = (
									<Autoselect
										key={name}
										{...absenceDetailsFieldData[name]}
										isRequired
										disableClearable
										value={values[name]}
										onChange={handleAutoselectChange(name)}
										options={options || []}
										errorText={errors[name]}
										error={Boolean(errors[name] && touched[name])}
										getKey={extractKeyById}
										disabled={disableAutoselectFields}
									/>
								);

								return withWrapper &&
									absenceDetailsFieldData[name]?.description ? (
									<FieldWithDescription
										key={name}
										description={absenceDetailsFieldData[name].description}
									>
										{field}
									</FieldWithDescription>
								) : (
									field
								);
							}
						)}
						<TextAreaField
							label={comment.label}
							name={`details.${comment.name}`}
							size="fullWidth"
							minRows={MIN_COMMENT_COUNT_OF_ROWS}
							value={values.comment || ''}
							onChange={handleTextAreaChange}
							error={Boolean(errors.comment && touched.comment)}
							errorText={errors.comment}
						/>
					</FormWrapper>
				</FormSection>
				<FormSection title={limits}>
					{filteredCheckBoxes.map(({ label, description, name }) => (
						<Checkbox
							key={name}
							{...{ name, wrapperStyles }}
							checked={!!values[name]}
							onClick={handleCheckBoxClick(name)}
							label={
								<FieldWithDescription {...{ description }} gapSize={spaces.xs}>
									<CheckboxLabel variant="m-400">{label}</CheckboxLabel>
								</FieldWithDescription>
							}
						/>
					))}
				</FormSection>
			</FormWrapper>
		</Form>
	);
}

export const DetailsTab = DetailsTabInner;
