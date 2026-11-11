import { useContext } from 'react';

import { InfoDataContainer } from '@/components/InfoDataContainer';
import { TabContext } from '@/components/UserProfile/context/tabContext';

import { SectionGridWrapper } from './styles';
import { SectionGridContainerProps } from './types';

export function SectionGridContainer({
	children,
	...props
}: SectionGridContainerProps) {
	const { handleTabChange } = useContext(TabContext);

	const handleClickView = () => {
		const { tabId } = props;

		if (tabId) {
			handleTabChange(tabId);
		}
	};

	return (
		<InfoDataContainer {...props} handleClickView={handleClickView}>
			<SectionGridWrapper>{children}</SectionGridWrapper>
		</InfoDataContainer>
	);
}
