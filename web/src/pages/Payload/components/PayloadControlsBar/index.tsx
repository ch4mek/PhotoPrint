import { useTheme } from '@emotion/react';

import { FilterAutoselect } from '@/components/FilterAutoselect';
import { FilterDropdown } from '@/components/FilterDropdown';
import { checkIfPrimitiveOptionsEqual } from '@/helpers/checkIfOptionEqualToValue';
import { extractKeyFromPrimitive } from '@/helpers/extractKey';
import { PayloadControlsBarProps } from '@/pages/Payload/components/PayloadControlsBar/types';
import { INITIAL_SPECIALIZATION_TITLE } from '@/pages/Payload/contants';

export function PayloadControlsBar({
	year,
	specializationOptions,
	yearsOptions,
	handleYearChange,
	handleSpecializationChange,
	specializationTitle,
	isMobileView,
}: PayloadControlsBarProps) {
	const theme = useTheme();

	const fieldStylesConfig = {
		backgroundColor: theme.palette['background-white'],
	};

	const handleSearchChange = () => {};

	return (
		<>
			<FilterDropdown
				nameField="Year"
				value={year}
				options={yearsOptions}
				stylesConfig={fieldStylesConfig}
				onChange={handleYearChange}
				isMobileView={isMobileView}
			/>
			<FilterAutoselect
				label=""
				value={specializationTitle}
				options={specializationOptions}
				onChange={handleSpecializationChange}
				valueToClean={INITIAL_SPECIALIZATION_TITLE}
				getKey={extractKeyFromPrimitive}
				checkIfOptionEqualToValue={checkIfPrimitiveOptionsEqual}
				stylesConfig={fieldStylesConfig}
				preventDropdownFlip
				onSearchInputChange={handleSearchChange}
				isMobileView={isMobileView}
			/>
		</>
	);
}
