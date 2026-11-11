import { AxiosError } from 'axios';
import { FormikHelpers } from 'formik';

import { MAIN_COUNTRY_NAME } from '../UserForm/constants';

import {
	getIsAlreadyUsedMessage,
	RESPONSE_EMAIL_EXIST,
} from '@/constants/errors';
import { EMAIL_FIELDS } from '@/constants/user';
import { useAuth } from '@/context/AuthContext';
import { useConfiguration } from '@/context/Configuration';
import { FormikWithBackup } from '@/forms/components/FormikWithBackup';
import {
	checkIfMessageIsAboutEmailDuplicate,
	checkIfMessageIsAboutPhoneDuplicate,
	checkIfMessageIsAboutRoleDuplicate,
} from '@/helpers/checkIfMessageIsAboutDuplicate';
import { getEmailDuplicateFields } from '@/helpers/getEmailDuplicateFields';
import { ServerResponseError } from '@/queries/types';

import { UserAddingFormContent } from './components/UserAddingFormContent';
import {
	ADMIN_PANEL_USER_FORM_FIELDS,
	ADMIN_PANEL_USER_INITIAL_VALUES,
	FORM_NAME,
} from './constants';
import { validationSchema } from './schema';
import { AdminPanelUserAddingFormValues, UserAddingFormProps } from './types';
import { convertDataToSubmit } from './utils/convertDataToSubmit';

export function UserAddingForm({
	hasHRModuleClaim,
	isLoading,
	onClose,
	onSubmit,
}: UserAddingFormProps) {
	const { role } = useAuth();
	const {
		configurableValues: { countries },
	} = useConfiguration();

	const handleSubmit = async (
		values: AdminPanelUserAddingFormValues,
		{
			setFieldError,
			setFieldTouched,
		}: FormikHelpers<AdminPanelUserAddingFormValues>
	) => {
		const countryId =
			countries.find(({ name }) => name === MAIN_COUNTRY_NAME)?.id ?? 0;

		try {
			await onSubmit({ ...convertDataToSubmit(values), countryId });
			onClose();
		} catch (error) {
			const message = (
				error as AxiosError<ServerResponseError>
			)?.response?.data?.Message?.trim();

			if (checkIfMessageIsAboutPhoneDuplicate(message)) {
				const { name, label } = ADMIN_PANEL_USER_FORM_FIELDS.personalPhone;
				setFieldError(name, getIsAlreadyUsedMessage(label, false));
			}

			if (checkIfMessageIsAboutEmailDuplicate(message)) {
				getEmailDuplicateFields(message, values, EMAIL_FIELDS).forEach(
					(field) => setFieldError(field, RESPONSE_EMAIL_EXIST)
				);
			}

			if (checkIfMessageIsAboutRoleDuplicate(message)) {
				setFieldError(ADMIN_PANEL_USER_FORM_FIELDS.position.name, message);
				setFieldTouched(
					ADMIN_PANEL_USER_FORM_FIELDS.position.name,
					true,
					false
				);
				setFieldError(ADMIN_PANEL_USER_FORM_FIELDS.department.name, message);
				setFieldTouched(
					ADMIN_PANEL_USER_FORM_FIELDS.department.name,
					true,
					false
				);
			}
		}
	};

	return (
		<FormikWithBackup
			enableReinitialize
			validationSchema={validationSchema}
			onSubmit={handleSubmit}
			onClose={onClose}
			validateOnBlur={false}
			name={FORM_NAME}
			initialValues={ADMIN_PANEL_USER_INITIAL_VALUES}
			validationContext={{ role, hasHRModuleClaim }}
			id={FORM_NAME}
		>
			{({ handleSubmit: onFormikSubmit, handleOutsideClose }) => (
				<UserAddingFormContent
					{...{
						hasHRModuleClaim,
						onClose,
						isLoading,
						handleOutsideClose,
					}}
					onSubmit={onFormikSubmit}
				/>
			)}
		</FormikWithBackup>
	);
}
