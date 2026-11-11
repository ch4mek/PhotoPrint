import { useContext } from 'react';

import { PersonalDataContext } from '../../context/personalDataContext';

import { Loader } from '@/components/Loader';
import { useOpenPopUpViaUrl } from '@/components/PopUpUrlInterceptor/hooks/useOpenPopUpViaUrl';
import { TabTableWrapper } from '@/components/TabWrapper';
import { PopUpSearchParams } from '@/constants/searchParams';
import { AbsenceHistoryProvider } from '@/context/Absence';
import { useToast } from '@/context/Toast';
import { createOnErrorHandler } from '@/helpers/requestErrorHandler';
import { useAbsenceRequestGridState } from '@/hooks/useAbsenceRequestGridState';
import { AbsenceHistoryHeader } from '@/pages/TeamBoardUser/components/AbsencesTab/components/AbsenceHistoryHeader';
import { AbsencesTabWrapper } from '@/pages/TeamBoardUser/components/AbsencesTab/styles';
import { useGetUserAbsences } from '@/queries/absences/useGetUserAbsences';
import { AbsenceRequestGridRow } from '@/types/absences';

import { AbsenceItemList } from './components/AbsenceItemList';
import { AbsenceRequests } from './components/AbsenceRequests';
import { AbsenceTransactionHistory } from './components/AbsenceTransactionHistory';
import { useTransactionsHistoryGridState } from './hooks/useTransactionsHistoryGridState';

function AbsencesTabInner() {
	const toast = useToast();
	const {
		userId,
		accessLevel,
		data: { registeredLocationId },
	} = useContext(PersonalDataContext);

	const { data, isLoading } = useGetUserAbsences(userId!, {
		enabled: !!userId,
		onError: createOnErrorHandler(toast),
	});

	const absenceRequestGridState = useAbsenceRequestGridState({
		numberOfAbsences: data?.length || 0,
		userId: userId!,
	});

	const { totalUsed, ...transactionsHistoryGridState } =
		useTransactionsHistoryGridState();

	const isHasFirstRow = !!data?.length;

	const handleOpenPopUp = useOpenPopUpViaUrl(PopUpSearchParams.InquiryIdParam);

	const handleOpen = (row: AbsenceRequestGridRow) => {
		handleOpenPopUp(row.id.toString());
	};

	return (
		<Loader {...{ isLoading }} size="small">
			<AbsencesTabWrapper>
				{isHasFirstRow && (
					<AbsenceItemList
						data={data || []}
						{...{ userId, accessLevel }}
						locationId={registeredLocationId}
					/>
				)}
				<AbsenceRequests
					handleOpen={handleOpen}
					absencesOptions={data || []}
					{...absenceRequestGridState}
				/>
				<TabTableWrapper>
					<AbsenceHistoryHeader absences={data || []} totalUsed={totalUsed} />
					<AbsenceTransactionHistory {...transactionsHistoryGridState} />
				</TabTableWrapper>
			</AbsencesTabWrapper>
		</Loader>
	);
}

export function AbsencesTab() {
	return (
		<AbsenceHistoryProvider>
			<AbsencesTabInner />
		</AbsenceHistoryProvider>
	);
}
