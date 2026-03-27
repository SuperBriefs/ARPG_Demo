using UnityEngine;

public class IdleState : State<EnemyController>
{
    private EnemyController enemy;
    private int nowCount = 0;

    public override void Enter(EnemyController owner)
    {
        enemy = owner;
        
        enemy.Animator.SetBool("combatMode", false);

        //进入巡逻状态
        enemy.NavAgent.speed = 2.0f;
    }

    public override void Execute()
    {
        //选取在视角内最近的目标追击
        enemy.Target = enemy.FindTarget();
        if(enemy.Target != null)
        {
            //追击前先警示周围敌人
            enemy.AlertNearbyEnemies();
            enemy.ChangeState(EnemyStates.CombatMovement);
        }
        else
        {
            //没有目标就巡逻
            if(enemy.patrolPoints.Count > 0)
            {
                enemy.Animator.SetFloat("forwardSpeed", 0.2f);
                //如果没有路径或者已经到达目标点
                if (!enemy.NavAgent.pathPending && 
                    enemy.NavAgent.remainingDistance <= enemy.NavAgent.stoppingDistance)
                {
                    //切换到下一个点
                    nowCount = (nowCount + 1) % enemy.patrolPoints.Count;
                    enemy.NavAgent.SetDestination(enemy.patrolPoints[nowCount]);
                }
            }
        }
    }

    public override void Exit()
    {
        //退出巡逻状态
        enemy.NavAgent.speed = 5f;
    }
}
