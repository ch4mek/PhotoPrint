import { useTheme } from '@emotion/react';
import { Checkbox } from '@mms/mms-ui-library/components/Checkbox';
import { PickerButtonOption } from '@mms/mms-ui-library/components/PickerButton';
import { Typography } from '@mms/mms-ui-library/components/Typography';

import { PER_USER_EXPORT_LABEL } from './constants';
import {
	OptionsSection,
	SelectionModalWrapper,
	SelectsSections,
	Option,
} from './styles';
import { ExportModalProps } from './types';

export function ExportModal<T>({
	onPerUserExportToggle,
	onChange,
	options,
	isPerUserExport,
	closePopover,
}: ExportModalProps<T>) {
	const theme = useTheme();

	const handleChange = (option: PickerButtonOption<T>) => () => {
		onChange?.(option);
		closePopover();
	};

	return (
		<SelectionModalWrapper>
			{onPerUserExportToggle && (
				<SelectsSections>
					<Checkbox
						label={PER_USER_EXPORT_LABEL}
						onClick={onPerUserExportToggle}
						checked={isPerUserExport}
					/>
				</SelectsSections>
			)}
			<OptionsSection>
				{options.map(({ value }) => (
					<Option key={value} onClick={handleChange({ value })}>
						<Typography variant="m-400" color={theme.palette['text-black']}>
							{value}
						</Typography>
					</Option>
				))}
			</OptionsSection>
		</SelectionModalWrapper>
	);
}
