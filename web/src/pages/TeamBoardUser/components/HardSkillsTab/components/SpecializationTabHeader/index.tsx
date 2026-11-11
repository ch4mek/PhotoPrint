import { Button } from '@mms/mms-ui-library/components/Button';

import { specializationHeaderButtons } from '@/pages/TeamBoardUser/components/HardSkillsTab/components/SpecializationTabHeader/constants';

import {
	EntryButtonWrapper,
	SpecializationButtonWrapper,
	TabHeaderWrapper,
} from './styles';
import { SpecializationTabHeaderProps } from './types';

const { addSpecialization, addNewEntry } = specializationHeaderButtons;

export function SpecializationTabHeader({
	onEntryAddClick,
	onSpecializationAddClick,
	isEntryButtonDisabled,
	isButtonDisabled,
}: SpecializationTabHeaderProps) {
	return (
		<TabHeaderWrapper>
			{onEntryAddClick && (
				<EntryButtonWrapper>
					<Button
						view="outline"
						onClick={onEntryAddClick}
						disabled={isEntryButtonDisabled || isButtonDisabled}
					>
						{addNewEntry}
					</Button>
				</EntryButtonWrapper>
			)}
			{onSpecializationAddClick && (
				<SpecializationButtonWrapper>
					<Button
						onClick={onSpecializationAddClick}
						disabled={isButtonDisabled}
					>
						{addSpecialization}
					</Button>
				</SpecializationButtonWrapper>
			)}
		</TabHeaderWrapper>
	);
}
