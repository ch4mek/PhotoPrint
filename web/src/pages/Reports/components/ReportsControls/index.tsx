import { FilterButton } from '@mms/mms-ui-library/components/FilterButton';
import { PickerButtonOption } from '@mms/mms-ui-library/components/PickerButton';
import { Popover } from '@mms/mms-ui-library/components/Popover';
import { useHasScreenWidth } from '@mms/mms-ui-library/hooks/useHasScreenWidth';
import { useOutsideClickMany } from '@mms/mms-ui-library/hooks/useOutsideClickMany';
import { useQueryClient } from '@tanstack/react-query';
import { memo, useCallback, useEffect, useRef, useState } from 'react';

import { CONTROLLERS_BREAKPOINT } from '../../constants';
import { ReportsControlsBar } from '../ReportsControlsBar';

import { ExportControl } from '@/components/ExportControl';
import { SAME_REPORT_GENERATING } from '@/constants/errors';
import { exportAsConfig } from '@/constants/exportConfigs';
import { NotificationType } from '@/constants/notifications';
import { ReportsQueriesKeys } from '@/constants/queriesKeys';
import { useConfiguration } from '@/context/Configuration';
import { useToast } from '@/context/index';
import { convertServerToastType } from '@/helpers/convertServerToastType';
import { createToast } from '@/helpers/createToast';
import { useExport } from '@/queries/useExport';

import { MODAL_WIDTH, popoverOffset } from './constants';
import { ControlsWrapper } from './styles';
import type { ReportControlsProps } from './types';
import { useReportsFilters } from './useReportsFilters';

function ReportControlsComponent(props: ReportControlsProps) {
	const [isPopoverOpen, setIsPopoverOpen] = useState(false);
	const [isPerUserExport, setIsPerUserExport] = useState(false);
	const filterMenuButtonRef = useRef(null);
	const popoverRef = useRef(null);
	const { isLoading: loading } = useConfiguration();
	const { date, setDate, showToast, queryParams, setQueryParams, role } = props;

	const reportsFilters = useReportsFilters({
		date,
		setDate,
		showToast,
		setQueryParams,
		role,
	});

	const handleClosePopover = useCallback(() => {
		setIsPopoverOpen(false);
	}, []);

	useOutsideClickMany([popoverRef, filterMenuButtonRef], handleClosePopover);

	const handleOnChangeExportField = ({ value }: PickerButtonOption) => {
		if (value) {
			setQueryParams((prevQueryParams) => ({
				...prevQueryParams,
				file: value,
			}));
		}
	};

	const handlePerUserExportChange = () => {
		setIsPerUserExport((prevIsPerUserExport) => !prevIsPerUserExport);
	};

	const isDesktopWidth = useHasScreenWidth(CONTROLLERS_BREAKPOINT);

	useEffect(() => {
		if (isDesktopWidth) {
			handleClosePopover();
		}
	}, [isDesktopWidth]);

	const handleClickOnFilterMenuButton = useCallback(() => {
		setIsPopoverOpen((prevValue) => !prevValue);
	}, []);

	const toast = useToast();

	const queryClient = useQueryClient();

	const { isFetched } = useExport(
		{
			...queryParams,
			isPerUserExport,
		},
		{
			onSuccess: ({ toastType, body }) => {
				toast.open(createToast(convertServerToastType(toastType), body));
				queryClient.removeQueries({
					queryKey: [
						ReportsQueriesKeys.reportsDocument,
						queryParams,
						queryParams.file,
					],
				});
			},
			onError() {
				toast.open(createToast(NotificationType.INFO, SAME_REPORT_GENERATING));
			},
		}
	);

	useEffect(() => {
		if (isFetched) {
			setQueryParams((prevQueryParams) => ({
				...prevQueryParams,
				file: undefined,
			}));
		}
	}, [isFetched]);

	if (loading) {
		return null;
	}

	return (
		<ControlsWrapper>
			{isDesktopWidth ? (
				<ReportsControlsBar
					isMobileView={!isDesktopWidth}
					{...props}
					{...reportsFilters}
				/>
			) : (
				<>
					<Popover
						open={isPopoverOpen}
						anchorElement={filterMenuButtonRef.current}
						position="bottom"
						ref={popoverRef}
						offset={popoverOffset}
					>
						<ReportsControlsBar
							isMobileView={!isDesktopWidth}
							handleClosePopover={handleClosePopover}
							{...props}
							{...reportsFilters}
						/>
					</Popover>

					<FilterButton
						ref={filterMenuButtonRef}
						size="big"
						isActive={!reportsFilters.isResetDisabled}
						isOpen={isPopoverOpen}
						onClick={handleClickOnFilterMenuButton}
					/>
				</>
			)}

			<ExportControl
				options={exportAsConfig}
				onChange={handleOnChangeExportField}
				onPerUserExportToggle={handlePerUserExportChange}
				modalWidth={MODAL_WIDTH}
				{...{ isPerUserExport }}
			/>
		</ControlsWrapper>
	);
}

export const ReportControls = memo(ReportControlsComponent);
