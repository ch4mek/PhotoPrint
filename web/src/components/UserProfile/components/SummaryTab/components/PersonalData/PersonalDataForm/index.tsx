import { Autoselect } from '@mms/mms-ui-library/components/Autoselects';
import { PopUp } from '@mms/mms-ui-library/components/PopUp';
import { FormikHelpers } from 'formik';
import { useCallback, useMemo } from 'react';
import { useTranslation } from 'react-i18next';

import { FormikWithBackup } from '@/forms/components/FormikWithBackup';
import { LocationType } from '@/types/configuration';

import { validationSchema } from './constants';
import { PersonalDataFormProps, PersonalDataFormValues } from './types';

export function PersonalDataForm({
	handleClose,
	initialLocation,
	locationTypes,
	onSubmit,
	userId,
}: PersonalDataFormProps) {
	const initialValues: PersonalDataFormValues = useMemo(
		() => ({
			location:
				locationTypes.find(({ name }) => name === initialLocation) ?? null,
		}),
		[locationTypes, initialLocation]
	);
	const { t } = useTranslation('profile', { keyPrefix: 'personalData.popup' });
	const { t: tCommon } = useTranslation('common', {
		keyPrefix: 'controlButtons',
	});

	const handleSubmit = useCallback(
		async (
			{ location }: PersonalDataFormValues,
			{ setSubmitting }: FormikHelpers<PersonalDataFormValues>
		) => {
			if (location) {
				await onSubmit({ location: location.name, userId });
				setSubmitting(false);
				handleClose();
			}
		},
		[]
	);

	return (
		<FormikWithBackup
			enableReinitialize
			initialValues={initialValues}
			validationSchema={validationSchema}
			validateOnBlur={false}
			onClose={handleClose}
			onSubmit={handleSubmit}
			name={t('formName')}
			id={userId}
		>
			{({
				values,
				setFieldValue,
				setFieldTouched,
				validateField,
				errors,
				touched,
				submitForm,
				isSubmitting,
				handleOutsideClose,
			}) => {
				const { location } = values;

				const handleAutoselectChange = (value: LocationType | null) => {
					setFieldValue('location', value);
					setFieldTouched('location', true);
					validateField('location');
				};

				return (
					<PopUp
						title={t('title')}
						onClose={handleClose}
						onShadowClick={handleOutsideClose}
						type="form"
						controls={{
							positiveControl: { onClick: submitForm, text: tCommon('apply') },
							negativeControl: {
								onClick: handleClose,
								text: tCommon('cancel'),
							},
						}}
						showLoader={isSubmitting}
					>
						<Autoselect
							label={t('inputLabel')}
							value={location}
							options={locationTypes}
							onChange={handleAutoselectChange}
							isRequired
							valueField="description"
							error={Boolean(errors.location) && touched.location}
							errorText={errors.location}
						/>
					</PopUp>
				);
			}}
		</FormikWithBackup>
	);
}
