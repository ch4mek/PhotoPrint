import { Autoselect } from '@mms/mms-ui-library/components/Autoselects';

import { formSections } from '../../constants';
import { FormColumn } from '../../styles';

import { FormSection } from '@/components/FormSection';
import { useConfiguration } from '@/context/Configuration';
import { TextField } from '@/fields/Text';
import { ADMIN_PANEL_USER_FORM_FIELDS } from '@/forms/AdminPanelForms/UserAddingForm/constants';
import { useUserForm } from '@/forms/AdminPanelForms/UserAddingForm/hooks/useUserForm';

export function LocationSection() {
	const { values, errors, touched, setFieldValue, handleFieldChange } =
		useUserForm();

	const {
		persistentValues: { locationTypes },
		configurableValues: { countries },
	} = useConfiguration();

	return (
		<FormSection title={formSections.location}>
			<FormColumn>
				<TextField
					{...ADMIN_PANEL_USER_FORM_FIELDS.currentLocation}
					onChange={setFieldValue}
				/>
				<Autoselect
					{...ADMIN_PANEL_USER_FORM_FIELDS.officeLocation}
					value={values.officeLocation ?? null}
					onChange={handleFieldChange(
						ADMIN_PANEL_USER_FORM_FIELDS.officeLocation.name
					)}
					options={locationTypes}
					valueField="name"
					error={Boolean(errors.officeLocation) && touched.officeLocation}
					errorText={errors.officeLocation}
					isRequired
				/>
				<Autoselect
					{...ADMIN_PANEL_USER_FORM_FIELDS.registeredLocation}
					value={values.registeredLocation ?? null}
					onChange={handleFieldChange(
						ADMIN_PANEL_USER_FORM_FIELDS.registeredLocation.name
					)}
					options={countries}
					valueField="name"
					error={
						Boolean(errors.registeredLocation) && touched.registeredLocation
					}
					errorText={errors.registeredLocation}
					isRequired
				/>
			</FormColumn>
		</FormSection>
	);
}
