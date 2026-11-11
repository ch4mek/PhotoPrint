import { useTheme } from '@emotion/react';
import { Autoselect } from '@mms/mms-ui-library/components/Autoselects';
import { Chip } from '@mms/mms-ui-library/components/Chip';
import { useFormikContext } from 'formik';
import { useCallback, useMemo } from 'react';

import { microfrontendsFieldData } from '../../constants';
import { MicrofrontendsFormValues } from '../../types';
import { ChipArrayField } from '../ChipArrayField';
import { ChipsContainer } from '../ChipArrayField/styles';

import { TextField } from '@/fields/Text';
import { FormWrapper } from '@/forms/styles';
import { UserClaims } from '@/types/claims';

import { AutoselectWrapper, ChipsWrapper } from './styles';
import { validateModule, validateRoute } from './utils/validateChips';

export function MicrofrontendsFormContent() {
	const theme = useTheme();

	const { values, setFieldTouched, setFieldValue } =
		useFormikContext<MicrofrontendsFormValues>();

	const fieldValues: string[] =
		values[microfrontendsFieldData.claims.name] || [];

	const claimOptions = useMemo(
		() =>
			Object.values(UserClaims).filter(
				(scp) => typeof scp === 'string'
			) as string[],
		[]
	);

	const filteredClaimOptions = useMemo(
		() => claimOptions.filter((opt) => !fieldValues.includes(opt)),
		[claimOptions, fieldValues]
	);

	const handleAddClaim = useCallback(
		(selected: string[] | null) => {
			const newValues = [...fieldValues, selected];
			setFieldValue(microfrontendsFieldData.claims.name, newValues);
			setFieldTouched(microfrontendsFieldData.claims.name, true);
		},
		[fieldValues, setFieldValue, setFieldTouched]
	);

	const handleRemove = useCallback(
		(indexToRemove: number) => {
			const newValues = fieldValues.filter(
				(_, index) => index !== indexToRemove
			);
			setFieldValue(
				microfrontendsFieldData.claims.name,
				newValues.length > 0 ? newValues : null
			);
			setFieldTouched(microfrontendsFieldData.claims.name, true);
		},
		[
			fieldValues,
			microfrontendsFieldData.claims.name,
			setFieldValue,
			setFieldTouched,
		]
	);

	return (
		<FormWrapper>
			<TextField
				{...microfrontendsFieldData.name}
				onChange={setFieldValue}
				isRequired
			/>

			<TextField {...microfrontendsFieldData.scope} onChange={setFieldValue} />

			<ChipArrayField
				{...microfrontendsFieldData.modules}
				validation={validateModule}
			/>

			<TextField
				{...microfrontendsFieldData.remoteEntries}
				onChange={setFieldValue}
				isRequired
			/>

			<ChipsWrapper>
				<ChipArrayField
					{...microfrontendsFieldData.routes}
					validation={validateRoute}
				/>

				<AutoselectWrapper>
					<Autoselect
						{...microfrontendsFieldData.claims}
						options={filteredClaimOptions as unknown as string[][]}
						value={null}
						onChange={handleAddClaim}
					/>

					{fieldValues.length > 0 && (
						<ChipsContainer>
							{fieldValues.map((value, index) => (
								<Chip
									key={`${value}-${index}`}
									labelStyle={{ padding: `${theme.spaces.xs}px` }}
									onDelete={() => handleRemove(index)}
									label={value}
									withCrossIcon
								/>
							))}
						</ChipsContainer>
					)}
				</AutoselectWrapper>
			</ChipsWrapper>

			<TextField
				{...microfrontendsFieldData.version}
				onChange={setFieldValue}
			/>
		</FormWrapper>
	);
}
