import { useContext, useMemo } from 'react';

import { AbsenceTable } from '../../AbsenceTable';
import { PersonalDataContext } from '../../../context/personalDataContext';
import { TeamBoardTabsId } from '../../../types';
import { TeamboardSectionContainer } from '../TeamboardSectionContainer';

import { InfoDataTableWrapper } from '@/components/InfoDataContainer/InfoDataTableWrapper';
import { useGetUserAbsences } from '@/queries/absences/useGetUserAbsences';

import { SECTION_HEADER } from './constants';
import { mapAbsencesToRows } from './utils';

export function AbsenceSection() {
	const { userId } = useContext(PersonalDataContext);

	const { data = [] } = useGetUserAbsences(userId ?? '', {
		enabled: !!userId,
	});

	const absenceTableRows = useMemo(() => {
		if (!data) {
			return [];
		}

		return mapAbsencesToRows(data);
	}, [data]);

	return (
		<TeamboardSectionContainer
			headerTitle={SECTION_HEADER}
			tabId={TeamBoardTabsId.ABSENCES}
		>
			<InfoDataTableWrapper>
				<AbsenceTable rows={absenceTableRows} />
			</InfoDataTableWrapper>
		</TeamboardSectionContainer>
	);
}
