import { ActionButton } from '@mms/mms-ui-library/components/ActionButton';
import { Tooltip } from '@mms/mms-ui-library/components/Tooltip';
import { CrossIcon } from '@mms/mms-ui-library/icons/base/CrossIcon';

import {
	AttachedFileText,
	AttachedFileTextWrapper,
	AttachedFileWrapper,
} from '@/components/AttachedFile/styles';
import { AttachedFileProps } from '@/components/AttachedFile/types';

export function AttachedFile({
	fileName,
	fileType,
	shouldShowFileType = true,
	handleRemoveClick,
}: AttachedFileProps) {
	return (
		<AttachedFileWrapper>
			<AttachedFileTextWrapper>
				{shouldShowFileType && fileType && (
					<Tooltip text={fileType} useIfTextCut>
						<AttachedFileText variant="m-600">{fileType}</AttachedFileText>
					</Tooltip>
				)}
				<Tooltip text={fileName} useIfTextCut>
					<AttachedFileText variant="m-400">{fileName}</AttachedFileText>
				</Tooltip>
			</AttachedFileTextWrapper>
			<ActionButton onClick={handleRemoveClick}>
				<CrossIcon />
			</ActionButton>
		</AttachedFileWrapper>
	);
}
