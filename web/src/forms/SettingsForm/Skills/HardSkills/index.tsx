import { Autoselect } from '@mms/mms-ui-library/components/Autoselects';
import { PopUp } from '@mms/mms-ui-library/components/PopUp';
import { FormikHelpers } from 'formik';
import { useCallback } from 'react';

import {
	getAddingPopUpTitle,
	getEditingPopUpTitle,
} from '@/constants/activityStatus';
import { useConfiguration } from '@/context/Configuration';
import { TextField } from '@/fields/Text';
import { FormikWithBackup } from '@/forms/components/FormikWithBackup';
import { FormWrapper } from '@/forms/styles';
import { useHardSkill } from '@/queries/settings/useHardSkill';
import { HardSkill } from '@/types/settings';

import { entityName, formName } from './constants';
import { validationSchema } from './schema';
import { HardSkillsFormProps, HardSkillsFormValues } from './types';

export function HardSkillsForm({
	initialValues,
	handleClose,
	onSubmit,
	id,
}: HardSkillsFormProps) {
	const {
		configurableValues: { hardSkillGroups },
	} = useConfiguration();

	const { data, isFetching } = useHardSkill(id);
	const handleSubmit = useCallback(
		async (
			values: HardSkillsFormValues,
			{ setSubmitting }: FormikHelpers<HardSkillsFormValues>
		) => {
			const isSubmited = await onSubmit({
				id,
				name: values.name,
				hardSkillGroupId:
					values.hardSkillGroupId?.hardSkillGroupId ||
					values.hardSkillGroupId?.id,
			} as unknown as HardSkill);

			setSubmitting(false);

			if (isSubmited) {
				handleClose();
			}
		},
		[]
	);

	const initialFormData = !data
		? initialValues
		: {
				name: data.name,
				hardSkillGroupId: {
					id: data.hardSkillGroupId,
					name: hardSkillGroups.find(
						(skill) => skill.id === data.hardSkillGroupId
					)?.name,
				},
		  };

	return (
		<FormikWithBackup
			enableReinitialize
			initialValues={initialFormData as HardSkillsFormValues}
			validationSchema={validationSchema}
			onSubmit={handleSubmit}
			onClose={handleClose}
			validateOnBlur={false}
			name={formName}
			id={id}
		>
			{({
				setFieldValue,
				submitForm,
				isSubmitting,
				values,
				errors,
				setFieldTouched,
				touched,
				handleOutsideClose,
			}) => {
				const onChangeHardSkillGroupId = (e: unknown) => {
					setFieldTouched('hardSkillGroupId', true);
					setFieldValue('hardSkillGroupId', e);
				};

				return (
					<PopUp
						title={
							id
								? getEditingPopUpTitle(entityName)
								: getAddingPopUpTitle(entityName)
						}
						onClose={handleClose}
						onShadowClick={handleOutsideClose}
						controls={{
							positiveControl: { onClick: submitForm },
							negativeControl: { onClick: handleClose },
						}}
						type="form"
						showLoader={Boolean(isFetching && id) || isSubmitting}
					>
						<FormWrapper>
							<TextField
								name="name"
								label="Hard skill"
								onChange={setFieldValue}
								isRequired
							/>
							<Autoselect
								value={values.hardSkillGroupId?.name}
								valueField="name"
								errorText={errors.hardSkillGroupId}
								error={
									Boolean(errors.hardSkillGroupId) && touched.hardSkillGroupId
								}
								label="Hard skill group"
								options={hardSkillGroups}
								onChange={onChangeHardSkillGroupId}
								isRequired
							/>
						</FormWrapper>
					</PopUp>
				);
			}}
		</FormikWithBackup>
	);
}
