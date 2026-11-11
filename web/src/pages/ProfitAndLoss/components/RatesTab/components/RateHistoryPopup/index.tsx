import { useTheme } from '@emotion/react';
import { PopUp } from '@mms/mms-ui-library/components/PopUp';
import { Tooltip } from '@mms/mms-ui-library/components/Tooltip';
import { useCallback, useMemo } from 'react';

import { TOOLTIP_FALLBACK_PLACEMENTS } from '@/components/Grid/constants';
import { InfoDataContainer } from '@/components/InfoDataContainer';
import { zIndexes } from '@/constants/zIndexes';
import { RateHistoryItemForm } from '@/forms/RateHistoryItemForm';
import { useResize } from '@/hooks/useResize';
import { useShowModal } from '@/pages/hooks/useShowModal';
import { useGetRateHistory } from '@/queries/rates/useGetRateHistory';

import { RateHistoryGrid } from './components/RateHistoryGrid';
import { RATE_HISTORY_GRID_HEADER_TITLE } from './components/RateHistoryGrid/constants';
import { RateHistoryTableWrapper } from './components/RateHistoryGrid/styles';
import { convertRateHistoryItemsIntoRows } from './components/RateHistoryGrid/utils/convertRateHistoryItemsIntoRows';
import {
	RATE_HISTORY_POPUP_TITLE,
	RATE_HISTORY_PROJECT_LABEL,
	RATE_HISTORY_USER_LABEL,
} from './constants';
import {
	RateHistoryInfoItem,
	RateHistoryInfoLabel,
	RateHistoryInfoRow,
	RateHistoryInfoValue,
	RateHistoryInfoValueWrapper,
	RateHistoryPopupContent,
	RateHistoryProjectColorCircle,
	RateHistoryProjectValue,
} from './styles';
import { RateHistoryPopupProps } from './types';
import { getPopUpStylesConfig } from './utils/getPopUpStylesConfig';
import { getRateHistoryItemFormItemFromRow } from './utils/getRateHistoryItemFormItemFromRow';

export function RateHistoryPopup({
	historyContext,
	onClose,
}: RateHistoryPopupProps) {
	const [, windowHeight] = useResize();
	const theme = useTheme();

	const { userId, projectId, userName, projectName, projectColor } =
		historyContext;

	const popUpStylesConfig = useMemo(
		() => getPopUpStylesConfig(theme, windowHeight),
		[theme, windowHeight]
	);

	const { data, isFetching, isLoading } = useGetRateHistory({
		userId,
		projectId,
	});

	const canAddRate = Boolean(data?.canRateBeAddedByCurrentUser);
	const isDataLoading = isLoading || isFetching;

	const {
		isOpen: isRateItemFormOpen,
		id: editingRateItemId,
		handleOpen: handleOpenRateItemForm,
		handleClose: handleCloseRateItemForm,
	} = useShowModal<number>();

	const handleAddRateHistoryItem = useCallback(() => {
		handleOpenRateItemForm();
	}, [handleOpenRateItemForm]);

	const handleRateHistoryItemFormOpen = useCallback(
		(rateItemId: number) => {
			handleOpenRateItemForm(rateItemId);
		},
		[handleOpenRateItemForm]
	);

	const handleEditRateHistoryItem = useCallback(
		(rateItemId?: string | number) => () => {
			if (rateItemId != null) {
				handleRateHistoryItemFormOpen(Number(rateItemId));
			}
		},
		[handleRateHistoryItemFormOpen]
	);

	const rows = useMemo(
		() =>
			convertRateHistoryItemsIntoRows(
				data?.projectRates ?? [],
				handleEditRateHistoryItem
			),
		[data?.projectRates, handleEditRateHistoryItem]
	);

	const editingRateItem = useMemo(() => {
		if (editingRateItemId == null) {
			return undefined;
		}

		const row = rows.find(({ id }) => id === editingRateItemId);

		return row ? getRateHistoryItemFormItemFromRow(row) : undefined;
	}, [editingRateItemId, rows]);

	const canOpenRateItemForm = useMemo(() => {
		if (!isRateItemFormOpen) {
			return false;
		}

		if (editingRateItemId == null) {
			return canAddRate;
		}

		const row = rows.find(({ id }) => id === editingRateItemId);

		return Boolean(row?.handleEditClick);
	}, [isRateItemFormOpen, editingRateItemId, canAddRate, rows]);

	return (
		<>
			<PopUp
				title={RATE_HISTORY_POPUP_TITLE}
				onClose={onClose}
				showCloseButton
				stylesConfig={popUpStylesConfig}
				showLoader={isDataLoading}
			>
				<RateHistoryPopupContent>
					<RateHistoryInfoRow>
						<RateHistoryInfoItem>
							<RateHistoryInfoLabel variant="m-400">
								{RATE_HISTORY_USER_LABEL}
							</RateHistoryInfoLabel>
							<RateHistoryInfoValueWrapper>
								<Tooltip
									zIndex={zIndexes.tooltip}
									text={userName}
									useIfTextCut
									fallbackPlacements={TOOLTIP_FALLBACK_PLACEMENTS}
								>
									<RateHistoryInfoValue>{userName}</RateHistoryInfoValue>
								</Tooltip>
							</RateHistoryInfoValueWrapper>
						</RateHistoryInfoItem>

						<RateHistoryInfoItem>
							<RateHistoryInfoLabel variant="m-400">
								{RATE_HISTORY_PROJECT_LABEL}
							</RateHistoryInfoLabel>
							<RateHistoryProjectValue>
								{projectColor && (
									<RateHistoryProjectColorCircle color={projectColor} />
								)}
								<RateHistoryInfoValueWrapper>
									<Tooltip
										zIndex={zIndexes.tooltip}
										text={projectName}
										useIfTextCut
										fallbackPlacements={TOOLTIP_FALLBACK_PLACEMENTS}
									>
										<RateHistoryInfoValue>{projectName}</RateHistoryInfoValue>
									</Tooltip>
								</RateHistoryInfoValueWrapper>
							</RateHistoryProjectValue>
						</RateHistoryInfoItem>
					</RateHistoryInfoRow>

					<InfoDataContainer
						headerTitle={RATE_HISTORY_GRID_HEADER_TITLE}
						handleAddClick={canAddRate ? handleAddRateHistoryItem : undefined}
					>
						<RateHistoryTableWrapper>
							<RateHistoryGrid {...{ rows }} />
						</RateHistoryTableWrapper>
					</InfoDataContainer>
				</RateHistoryPopupContent>
			</PopUp>

			{canOpenRateItemForm && (
				<RateHistoryItemForm
					key={editingRateItemId ?? 'add'}
					userId={Number(userId)}
					projectId={Number(projectId)}
					rateItem={editingRateItem}
					onClose={handleCloseRateItemForm}
				/>
			)}
		</>
	);
}
