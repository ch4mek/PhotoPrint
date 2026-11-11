import { useTheme } from '@emotion/react';

import { FilterAutoselect } from '@/components/FilterAutoselect';
import { ResetFilterButton } from '@/components/ResetFilterButton';

import { GROUP_NAME_FIELD, initialClaimsFilterValue } from './constants';
import { useClaimsFiltersController } from './hooks/useClamsFiltersController';
import { AutoselectWrapper, FiltersWrapper } from './styles';

export function UserClaimsFilters() {
	const {
		claimsOptions,
		selectedClaimOption,
		handleClaimChange,
		resetFilters,
		isResetDisabled,
	} = useClaimsFiltersController();
	const theme = useTheme();
	const autoselectStylesConfig = {
		backgroundColor: theme.palette['background-white'],
	};

	return (
		<FiltersWrapper>
			<AutoselectWrapper>
				<FilterAutoselect
					label=""
					valueField="name"
					value={selectedClaimOption}
					valueToClean={initialClaimsFilterValue}
					options={claimsOptions}
					onChange={handleClaimChange}
					groupByField={GROUP_NAME_FIELD}
					stylesConfig={autoselectStylesConfig}
				/>
			</AutoselectWrapper>
			<ResetFilterButton disabled={isResetDisabled} onClick={resetFilters} />
		</FiltersWrapper>
	);
}
