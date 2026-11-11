import { Loader } from '@/components/Loader';
import { TabTableWrapper } from '@/components/TabWrapper';
import { AbsenceHistoryProvider } from '@/context/Absence';
import { useAuth } from '@/context/AuthContext';
import { useToast } from '@/context/Toast';
import { createOnErrorHandler } from '@/helpers/requestErrorHandler';
import { useAbsenceRequestGridState } from '@/hooks/useAbsenceRequestGridState';
import { AbsenceItemList } from '@/pages/TeamBoardUser/components/AbsencesTab/components/AbsenceItemList';
import { useGetUserAbsences } from '@/queries/absences/useGetUserAbsences';

import { AbsenceHistoryHeader } from './components/AbsenceHistoryHeader';
import { AbsenceRequests } from './components/AbsenceRequests';
import { AbsenceTransactionHistory } from './components/AbsenceTransactionHistory';
import { useTransactionsHistoryGridState } from './hooks/useTransactionsHistoryGridState';

function AbsencesTabInner() {
	const toast = useToast();
	const { id: userId, registeredLocationId } = useAuth();

	const { data, isLoading } = useGetUserAbsences(userId, {
		enabled: !!userId,
		onError: createOnErrorHandler(toast),
	});

	const absenceRequestGridState = useAbsenceRequestGridState({
		numberOfAbsences: data?.length || 0,
	});

	const { totalUsed, ...transactionsHistoryGridState } =
		useTransactionsHistoryGridState();

	const isHasFirstRow = !!data?.length;

	return (
		<Loader {...{ isLoading }} size="small">
			<TabTableWrapper>
				{isHasFirstRow && (
					<AbsenceItemList
						data={data || []}
						{...{ userId }}
						locationId={registeredLocationId}
					/>
				)}
				<AbsenceRequests
					absencesOptions={data || []}
					{...absenceRequestGridState}
				/>
				<TabTableWrapper>
					<AbsenceHistoryHeader absences={data || []} totalUsed={totalUsed} />
					<AbsenceTransactionHistory {...transactionsHistoryGridState} />
				</TabTableWrapper>
			</TabTableWrapper>
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
