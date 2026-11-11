import { Autoselect } from '@mms/mms-ui-library/components/Autoselects';
import { DatePicker } from '@mms/mms-ui-library/components/CustomDatePicker';

import { formSections } from '../../constants';
import { FormColumn } from '../../styles';

import { FormSection } from '@/components/FormSection';
import { currentDate } from '@/constants/date';
import { militaryServiceStatusOptions } from '@/constants/user';
import { TextField } from '@/fields/Text';
import { ADMIN_PANEL_USER_FORM_FIELDS } from '@/forms/AdminPanelForms/UserAddingForm/constants';
import { useUserForm } from '@/forms/AdminPanelForms/UserAddingForm/hooks/useUserForm';
import dayjs from '@/lib/dayjs';

export function PersonalDetailsSection() {
	const { values, errors, touched, setFieldValue, handleFieldChange } =
		useUserForm();

	return (
		<FormSection title={formSections.personalDetails}>
			<FormColumn>
				<TextField
					{...ADMIN_PANEL_USER_FORM_FIELDS.lastName}
					onChange={setFieldValue}
					isRequired
				/>
				<TextField
					{...ADMIN_PANEL_USER_FORM_FIELDS.firstName}
					onChange={setFieldValue}
					isRequired
				/>
				<TextField
					{...ADMIN_PANEL_USER_FORM_FIELDS.lastNameRu}
					onChange={setFieldValue}
					isRequired
				/>
				<TextField
					{...ADMIN_PANEL_USER_FORM_FIELDS.firstNameRu}
					onChange={setFieldValue}
					isRequired
				/>
				<TextField
					{...ADMIN_PANEL_USER_FORM_FIELDS.patronymicRu}
					onChange={setFieldValue}
					isRequired
				/>
				<DatePicker
					innerLabel={ADMIN_PANEL_USER_FORM_FIELDS.dateOfBirth.label}
					size="fullWidth"
					errorText={touched.dateOfBirth ? errors.dateOfBirth : ''}
					value={dayjs(values.dateOfBirth)}
					onChange={handleFieldChange(
						ADMIN_PANEL_USER_FORM_FIELDS.dateOfBirth.name
					)}
					maxDate={currentDate}
					isRequired
				/>
				<Autoselect
					{...ADMIN_PANEL_USER_FORM_FIELDS.militaryServiceStatus}
					value={values.militaryServiceStatus ?? null}
					onChange={handleFieldChange(
						ADMIN_PANEL_USER_FORM_FIELDS.militaryServiceStatus.name
					)}
					options={militaryServiceStatusOptions}
					error={Boolean(errors.militaryServiceStatus)}
					errorText={errors.militaryServiceStatus}
				/>
			</FormColumn>
		</FormSection>
	);
}
