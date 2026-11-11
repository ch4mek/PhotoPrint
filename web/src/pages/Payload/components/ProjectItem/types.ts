import { PayloadProjectWithClientName } from '../../types';

type ProjectItemContentDirection = 'column' | 'row';

export interface ProjectDirectionProps {
	direction: ProjectItemContentDirection;
}

export interface ProjectItemProps extends ProjectDirectionProps {
	project: PayloadProjectWithClientName;
}
