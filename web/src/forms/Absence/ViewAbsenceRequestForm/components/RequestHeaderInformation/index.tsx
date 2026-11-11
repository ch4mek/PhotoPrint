import { useTheme } from '@emotion/react';
import { Tooltip } from '@mms/mms-ui-library/components/Tooltip';
import { Typography } from '@mms/mms-ui-library/components/Typography';
import { DotIcon } from '@mms/mms-ui-library/icons/base/DotIcon';

import { getCreatedLabel } from '../../utils/getCreatedLabel';
import { getFormattedRange } from '../../utils/getFormattedRange';

import { REQUEST_FOR_LABEL } from '@/constants/absence';
import { dotIconStyles } from '@/constants/commonStyles';
import { RequestStatus } from '@/forms/Absence/ViewAbsenceRequestForm/components/RequestStatus';
import { checkIsDocumentRequest } from '@/helpers/absences';
import { getFullName } from '@/helpers/utils';

import {
	Label,
	RequestHeaderDatesWrapper,
	RequestHeaderInformationWrapper,
	RequestHeaderInfoWrapper,
} from './styles';
import { RequestHeaderInformationProps } from './types';

export function RequestHeaderInformation({
	item,
	withStatus,
}: RequestHeaderInformationProps) {
	const theme = useTheme();
	const {
		createdForUser: { lastName, firstName },
		inquiryTemplateName,
		status,
		dateTo,
		dateFrom,
		createdAtUtc,
		inquiryType,
	} = item;

	const headerLabel = `${inquiryTemplateName} ${REQUEST_FOR_LABEL} ${getFullName(
		firstName,
		lastName
	)}`;

	return (
		<RequestHeaderInformationWrapper>
			<RequestHeaderInfoWrapper>
				<Tooltip
					useIfTextCut
					text={headerLabel}
					textCutOrientation="vertical"
					position="bottom"
				>
					<Label variant="m-600">{headerLabel}</Label>
				</Tooltip>
				{withStatus && <RequestStatus {...{ status }} />}
			</RequestHeaderInfoWrapper>

			<RequestHeaderDatesWrapper>
				{!checkIsDocumentRequest(inquiryType) && (
					<>
						<Typography variant="m-400">
							{getFormattedRange(dateFrom, dateTo)}
						</Typography>
						<DotIcon
							color={theme.palette['graphics-gray']}
							{...dotIconStyles}
						/>
					</>
				)}
				<Typography variant="m-400">{getCreatedLabel(createdAtUtc)}</Typography>
			</RequestHeaderDatesWrapper>
		</RequestHeaderInformationWrapper>
	);
}
