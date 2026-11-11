import { Outlet } from 'react-router-dom';

import { PageWrapper } from '../styles';

import { DraggableProvider } from '@/pages/Wiki/context/draggableContext';

import { GroupSection } from './components/GroupSection';
import { WikiProvider } from './context/wikiContext';
import { EditorWrapper, Wrapper } from './styles';

export function WikiPage() {
	return (
		<PageWrapper>
			<WikiProvider>
				<Wrapper>
					<DraggableProvider>
						<GroupSection />
					</DraggableProvider>
					<EditorWrapper>
						<Outlet />
					</EditorWrapper>
				</Wrapper>
			</WikiProvider>
		</PageWrapper>
	);
}
