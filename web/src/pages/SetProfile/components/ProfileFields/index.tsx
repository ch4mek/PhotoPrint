import { Autoselect } from '@mms/mms-ui-library/components/Autoselects/Autoselect';
import { useTheme } from '@mui/material';
import { memo, useCallback, useMemo } from 'react';

import { chiefRoles } from '@/constants/roles';
import { useAuth } from '@/context/AuthContext';
import { useConfiguration } from '@/context/Configuration';
import { TextField } from '@/fields/Text';
import { DepartmentType, LocationType } from '@/types/configuration';

import {
	DEPARTMENT_TEXT,
	FIRST_NAME_TEXT,
	LAST_NAME_TEXT,
	LOCATION_TEXT,
} from './constants';
import { Wrapper } from './styles';
import type { ProfileFieldsInnerProps } from './types';

function ProfileFieldsInner({
	values,
	errors,
	touched,
	setFieldValue,
	setFieldTouched,
	validateField,
}: ProfileFieldsInnerProps) {
	const {
		configurableValues: { departmentTypes },
		persistentValues: { locationTypes },
	} = useConfiguration();
	const { departmentId } = useAuth();
	const theme = useTheme();

	const handleAutoselectChange =
		(field: string) => (value: DepartmentType | LocationType | null) => {
			setFieldValue(field, value?.id || null);
			setFieldTouched(field, true);
			validateField(field);
		};

	const textFieldStylesConfig = useMemo(
		() => ({
			label: {
				background: theme.palette['background-white'],
			},
			errorText: {
				wrapper: {
					background: theme.palette['background-white'],
				},
			},
		}),
		[theme]
	);

	const getValue = useCallback(
		(valueId: string, options: { id: string }[]) =>
			options.find((option) => option.id === valueId) || null,
		[]
	);

	return (
		<Wrapper>
			<TextField
				label={FIRST_NAME_TEXT}
				name="firstName"
				onChange={setFieldValue}
				stylesConfig={textFieldStylesConfig}
				disabled
				isRequired
			/>
			<TextField
				label={LAST_NAME_TEXT}
				name="lastName"
				onChange={setFieldValue}
				stylesConfig={textFieldStylesConfig}
				disabled
				isRequired
			/>
			{!chiefRoles.includes(values.position) && (
				<Autoselect
					label={DEPARTMENT_TEXT}
					value={
						getValue(values.departmentId, departmentTypes) as DepartmentType
					}
					valueField="fullName"
					options={departmentTypes}
					onChange={handleAutoselectChange('departmentId')}
					error={Boolean(errors.departmentId) && touched.departmentId}
					errorText={errors.departmentId}
					disableClearable
					disabled={Boolean(departmentId)}
					labelAndErrorBgColor={theme.palette['background-white']}
					isRequired
				/>
			)}
			<Autoselect
				label={LOCATION_TEXT}
				value={getValue(values.location, locationTypes) as LocationType}
				valueField="description"
				options={locationTypes}
				onChange={handleAutoselectChange('location')}
				error={Boolean(errors.location) && touched.location}
				errorText={errors.location}
				disableClearable
				labelAndErrorBgColor={theme.palette['background-white']}
				isRequired
			/>
		</Wrapper>
	);
}

export const ProfileFields = memo(ProfileFieldsInner);
