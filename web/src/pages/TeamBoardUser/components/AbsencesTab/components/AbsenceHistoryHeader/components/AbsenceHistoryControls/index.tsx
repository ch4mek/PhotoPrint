import { useAbsenceHistoryFilters } from '../../hooks/useAbsenceHistoryFilters';
import { useGetAbsenceTypeTotalCount } from '../../hooks/useGetAbsenceTypeTotalCount';
import { TotalUsedTitle } from '../TotalUsedTitle';

import { FilterAutoselect } from '@/components/FilterAutoselect';
import { SelectionModalOption } from '@/components/Grid/renderers/header/types';
import {
	initialAbsenceFilterValue,
	initialYearFilterValue,
} from '@/constants/absence';
import { checkIfOptionsEqualById } from '@/helpers/checkIfOptionEqualToValue';
import { extractKeyById } from '@/helpers/extractKey';
import { SimpleConfigurableConfigurationItem } from '@/types/configuration';

import {
	AbsenceHistoryControlsWrapper,
	AutoselectsWrapper,
	FilterWrapper,
} from './styles';
import { AbsenceHistoryControlsProps } from './types';

export function AbsenceHistoryControls({
	absences,
	totalUsed,
}: AbsenceHistoryControlsProps) {
	const {
		handleAbsenceFilterChange,
		handleYearChange,
		yearFilter,
		absenceFilter,
		yearOptions,
		absenceTypesOptions,
	} = useAbsenceHistoryFilters(absences);

	const totalCount = useGetAbsenceTypeTotalCount({
		absences,
		absenceFilter,
		totalUsed,
	});

	return (
		<AbsenceHistoryControlsWrapper isHasTotalTitle={!!totalCount}>
			{totalCount && <TotalUsedTitle {...{ totalCount }} />}
			<AutoselectsWrapper>
				<FilterWrapper>
					<FilterAutoselect<SelectionModalOption>
						label=""
						valueField="description"
						value={yearFilter}
						options={yearOptions}
						onChange={handleYearChange}
						valueToClean={initialYearFilterValue}
						getKey={extractKeyById}
						checkIfOptionEqualToValue={checkIfOptionsEqualById}
						preventDropdownFlip
					/>
				</FilterWrapper>
				<FilterWrapper>
					<FilterAutoselect<SimpleConfigurableConfigurationItem>
						label=""
						valueField="name"
						value={absenceFilter}
						options={absenceTypesOptions}
						onChange={handleAbsenceFilterChange}
						valueToClean={initialAbsenceFilterValue}
						getKey={extractKeyById}
						checkIfOptionEqualToValue={checkIfOptionsEqualById}
						preventDropdownFlip
					/>
				</FilterWrapper>
			</AutoselectsWrapper>
		</AbsenceHistoryControlsWrapper>
	);
}
