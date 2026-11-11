import { Autoselect } from '@mms/mms-ui-library/components/Autoselects';
import { PopUp } from '@mms/mms-ui-library/components/PopUp';
import { FormikHelpers } from 'formik';
import { useCallback } from 'react';

import { PERSONAL_DATA_FORM_FIELDS } from '../PersonalData/constants';

import { FIELD_IS_REQUIRED } from '@/constants/errors';
import { useConfiguration } from '@/context/Configuration';
import { TextField } from '@/fields/Text';
import { FormikWithBackup } from '@/forms/components/FormikWithBackup';
import { FormWrapper } from '@/forms/styles';
import { CalendaryCountry, LocationType } from '@/types/configuration';

import { POPUP_TITLE, RegisteredLocationFields } from './constants';
import { validationSchema } from './schema';
import {
	RegisteredLocationFormProps,
	RegisteredLocationFormValues,
} from './types';

export function RegisteredLocationForm({
	handleClose,
	onSubmit,
	id,
	initialValues,
}: RegisteredLocationFormProps) {
	const {
		configurableValues: { countries },
		persistentValues: { locationTypes },
	} = useConfiguration();

	const handleSubmit = useCallback(
		async (
			values: RegisteredLocationFormValues,
			{ setSubmitting }: FormikHelpers<RegisteredLocationFormValues>
		) => {
			const isSubmited = await onSubmit({
				...values,
				registeredLocation: values.registeredLocation?.id as number,
				currentLocation: values.currentLocation?.trim()
					? values.currentLocation
					: null,
				location: values.location?.id as string,
				userId: id,
			});

			setSubmitting(false);

			if (isSubmited) {
				handleClose();
			}
		},
		[id, onSubmit, handleClose]
	);

	return (
		<FormikWithBackup
			enableReinitialize
			onSubmit={handleSubmit}
			onClose={handleClose}
			validateOnBlur={false}
			name={POPUP_TITLE}
			{...{ initialValues, validationSchema, id }}
		>
			{({
				setFieldValue,
				submitForm,
				isSubmitting,
				setFieldTouched,
				touched,
				values,
				errors,
				validateField,
				setFieldError,
				handleOutsideClose,
			}) => {
				const handleLocationChange =
					(fieldName: RegisteredLocationFields) =>
					async (value: LocationType | CalendaryCountry | null) => {
						await setFieldValue(fieldName, value, false);
						setFieldTouched(fieldName, true);
						validateField(fieldName);
					};

				const handleSubmitForm = () => {
					if (errors.currentLocation) {
						setFieldTouched(RegisteredLocationFields.CURRENT_LOCATION, true);

						return;
					}

					if (!values.location) {
						setFieldError(RegisteredLocationFields.LOCATION, FIELD_IS_REQUIRED);
						setFieldTouched(RegisteredLocationFields.LOCATION, true);

						return;
					}

					if (!values.registeredLocation) {
						setFieldError(
							RegisteredLocationFields.REGISTERED_LOCATION,
							FIELD_IS_REQUIRED
						);
						setFieldTouched(RegisteredLocationFields.REGISTERED_LOCATION, true);

						return;
					}

					submitForm();
				};

				return (
					<PopUp
						title={POPUP_TITLE}
						onClose={handleClose}
						onShadowClick={handleOutsideClose}
						controls={{
							positiveControl: { onClick: handleSubmitForm },
							negativeControl: { onClick: handleClose },
						}}
						type="form"
						showLoader={isSubmitting}
					>
						<FormWrapper>
							<TextField
								name={RegisteredLocationFields.CURRENT_LOCATION}
								label="Current location"
								onChange={setFieldValue}
							/>
							<Autoselect
								{...PERSONAL_DATA_FORM_FIELDS.location}
								value={values.location}
								valueField="name"
								options={locationTypes}
								onChange={handleLocationChange(
									RegisteredLocationFields.LOCATION
								)}
								error={Boolean(errors.location) && touched.location}
								errorText={errors.location}
								isRequired
							/>
							<Autoselect
								{...PERSONAL_DATA_FORM_FIELDS.registeredLocation}
								value={values.registeredLocation}
								valueField="name"
								options={countries}
								onChange={handleLocationChange(
									RegisteredLocationFields.REGISTERED_LOCATION
								)}
								error={
									Boolean(errors.registeredLocation) &&
									touched.registeredLocation
								}
								errorText={errors.registeredLocation}
								isRequired
							/>
						</FormWrapper>
					</PopUp>
				);
			}}
		</FormikWithBackup>
	);
}
