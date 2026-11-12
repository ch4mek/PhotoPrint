import { DropDownCoreProps } from '@/components/Dropdown/components/DropDownCore/types';
import { DropDownProps, SideActionConfig } from '@/components/Dropdown/types';
export type DropDownWithActionsProps = DropDownProps & {
    actions: SideActionConfig[];
    corePropsOverride?: Partial<DropDownCoreProps>;
    containerPropsOverride?: Partial<DropDownProps>;
};
